using shortid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using URLShortener.Data;

namespace URLShortener.Controllers
{
    public class HomeController : Controller
    {
        UserRepository userRepo = new UserRepository(Properties.Settings.Default.ConStr);
        URLShortenerRepository urlRepo = new URLShortenerRepository(Properties.Settings.Default.ConStr);

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Name = userRepo.GetByEmail(User.Identity.Name).Name;
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(User user, string password)
        {
            userRepo.AddUser(user, password);
            return RedirectToAction("Index");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            User user = userRepo.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            FormsAuthentication.SetAuthCookie(email, true);
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult ShortenURL()
        {
            return View();
        }
        [Authorize]
        public ActionResult ShortenURL(URL url)
        {
            IEnumerable<URL> urls = urlRepo.GetByUser(userRepo.GetByEmail(User.Identity.Name).Id);
            URL old = urls.FirstOrDefault(u => u.OriginalURL.ToLower() == url.OriginalURL.ToLower());
            if (old != null)
            {
                return Json($"{Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "")}/{old.ShortURL}", JsonRequestBehavior.AllowGet);
            }

            url.ShortURL = ShortId.Generate(true, false);
            url.UserId = userRepo.GetByEmail(User.Identity.Name).Id;
            urlRepo.AddURL(url);

            return Json($"{Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "")}/{url.ShortURL}", JsonRequestBehavior.AllowGet);   
        }
        [Authorize]
        public ActionResult History()
        {
            ViewBag.CurrentUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "");
            IEnumerable<URL> urls = urlRepo.GetByUser(userRepo.GetByEmail(User.Identity.Name).Id);
            return View(urls);
        }
        [Route("{url}")]
        public ActionResult RedirectToDifferentWebsite(string url)
        {
            URL u = urlRepo.GetByShortURL(url);
            if (u == null)
            {
                return Redirect("/");
            }
            urlRepo.UpdateViewsCount(u.Id);
            return Redirect(u.OriginalURL);
        }
    }
}