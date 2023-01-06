using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebFilm.Models;
namespace WebFilm.Controllers
{
    public class LoginController : Controller
    {
        private WebXemPhimEntities db = new WebXemPhimEntities();
        // GET: Login
        
        public ActionResult Index()
        {
            if (Session["username"] != "" && Session["username"] != null)
            {
                var films = db.Films.Include(f => f.Author).Include(f => f.Category).Include(f => f.Country).Include(f => f.Genre);
                var genres = from c in db.Genres select c;
                ViewBag.genres = genres.ToList();
                ViewBag.genresID = new SelectList(genres, "GenreID", "Genrename");
                ViewBag.genres = genres.ToList();
                return Redirect(Url.Action("Index", "Home", films.ToList()));
            }
            else 
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            Session["Timkiem"] = "-1";
            var histories = from c in db.Histories select c;
            ViewBag.history = histories.ToList();
            Double sotien = 0;
            for (int i = 1; i <= histories.Count(); i++)
            {
                var item = histories.FirstOrDefault(x => x.HistoryID == i);
                if (item != null)
                    sotien += (double)item.Money;
            }
            Session["Gain"] = sotien;
            var users = from d in db.Users select d;
            var listt = users.ToArray();
            int dem = 0;
            for (int i = 1; i <= listt.Length; i++)
            {
                var tmp = db.Histories.FirstOrDefault(x => x.UserID == i);
                if (tmp != null) dem++;
            }
            Session["NumBuy"] = dem;
            Session["Numdont"] = listt.Length - dem;
            Session["Percent"] = (float)dem / listt.Length * 100;
            //var histories = from c in db.Histories select c;
            ViewBag.history = histories.ToList();
            var userr = from s in db.Users select s;
            userr = userr.Where(s => s.Username.CompareTo(username) == 0);
            foreach (var item in userr)
            {
                if (item.Password.CompareTo(password) == 0)
                {
                    var categories = from c in db.Categories select c;
                    ViewBag.categoryID = new SelectList(categories, "CategoryID", "CategoryName");
                    Session["username"] = item.Username;
                    Session["accountname"] = item.Fullname;
                    Session["role"] = item.Status;
                    Session["userid"] = item.ID;
                    
                    Session["Able"] = "1";
                    ViewBag.error = "Good";
                    if (item.Status == false)
                    {
                        var films = db.Films.Include(f => f.Author).Include(f => f.Category).Include(f => f.Country).Include(f => f.Genre);
                        var genres = from c in db.Genres select c;
                        ViewBag.genres = genres.ToList();
                        ViewBag.genresID = new SelectList(genres, "GenreID", "Genrename");
                        ViewBag.genres = genres.ToList();
                         
                        return Redirect(Url.Action("Index", "Home", films.ToList())); }
                    else if (item.Status == true)
                        return Redirect(Url.Action("Dashboard", "Home" ));
                    else
                    {
                        var films = db.Films.Include(f => f.Author).Include(f => f.Category).Include(f => f.Country).Include(f => f.Genre);
                        var genres = from c in db.Genres select c;
                        ViewBag.genres = genres.ToList();
                        ViewBag.genresID = new SelectList(genres, "GenreID", "Genrename");
                        ViewBag.genres = genres.ToList();
                        return Redirect(Url.Action("Index", "Home", films.ToList()));
                    }
                }
            }
            Session["username"] = "";
            ViewBag.error = "Tên đăng nhập hoặc mật khẩu không hợp lệ";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("username"); Session.Remove("role"); Session.Remove("userid"); Session.Remove("accountname");
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Username,Password,Fullname,Email,Phone,Address,Status")] User user, string password)
        {
            
            user.Status = false;
            if (ModelState.IsValid && password.CompareTo(user.Password)==0)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if(password.CompareTo(user.Password) == 0) ViewBag.fail = "Nhập lại mật khẩu sai";
            ViewBag.error = "That bai";
            return View(user);
        }
    }
}