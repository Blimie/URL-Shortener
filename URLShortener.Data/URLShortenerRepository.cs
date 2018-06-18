using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Data
{
    public class URLShortenerRepository
    {
        private string _connectionString;

        public URLShortenerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddURL(URL u)
        {
            using (var context = new URLShortenerDataContext(_connectionString))
            {
                context.URLs.InsertOnSubmit(u);
                context.SubmitChanges();
            }
        }   
        public void UpdateViewsCount(int id)
        {
            using (var context = new URLShortenerDataContext(_connectionString))
            {                         
                context.ExecuteCommand("UPDATE URLs SET Views = Views + 1 WHERE Id = {0}", id);
            }
        }
        public URL GetByShortURL(string shortURL)
        {
            using (var context = new URLShortenerDataContext(_connectionString))
            {                  
                return context.URLs.FirstOrDefault(u => u.ShortURL == shortURL);
            }
        }
        public IEnumerable<URL> GetByUser(int userId)
        {
            using (var context = new URLShortenerDataContext(_connectionString))
            {
                return context.URLs.Where(u => u.UserId == userId).ToList();
            }
        }
    }
}
