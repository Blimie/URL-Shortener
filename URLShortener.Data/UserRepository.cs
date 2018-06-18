using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Data
{
    public class UserRepository
    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser(User user, string password)
        {
            string passwordSalt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, passwordSalt);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            using (var context = new URLShortenerDataContext(_connectionString))
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }
        }
        public User GetByEmail(string email)
        {
            using (var context = new URLShortenerDataContext(_connectionString))
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }
        public User Login(string email, string password)
        {
            User user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isCorrectPassword = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (isCorrectPassword)
            {
                return user;
            }
            return null;
        }
    }
}
