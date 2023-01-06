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
    public class HomeController : Controller
    {
        private WebXemPhimEntities db = new WebXemPhimEntities();
        public ActionResult Index(string SearchString, string Author, int genresID = 0,int categoryID=0)
        {
            var films = db.Films.Include(f => f.Author).Include(f => f.Category).Include(f => f.Country).Include(f => f.Genre);
            var genres = from c in db.Genres select c;
            ViewBag.genres = genres.ToList();
            ViewBag.genresID = new SelectList(genres, "GenreID", "Genrename");
            Session["Timkiem"] = "-1";
            if (!String.IsNullOrEmpty(SearchString))
            {
                films = films.Where(s => s.Filmname.Contains(SearchString));
                Session["Timkiem"] = "1";
            }
            if (!String.IsNullOrEmpty(Author))
            {
                films = films.Where(s => s.Author.Fullname.Contains(Author));
                Session["Timkiem"] = "1";
            }
            if (genresID != 0)
            {
                films = films.Where(s => s.GenreID == genresID);
                Session["Timkiem"] = "1";
            }
            if (categoryID != 0)
            {
                films = films.Where(s => s.CategoryID == categoryID);
                Session["Timkiem"] = "1";
            }
            //return View(films.ToList());
            if (Session["username"] != "" && Session["username"] != null) { 

                return View(films.ToList()); 
            }
            return View("~/Views/Login/Index.cshtml");
        }
        // GET: Films/Details/5
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Film film = db.Films.Find(id);
            if (film == null)
            {
                return HttpNotFound();
            }
            var history = db.Histories;
            var tmp = history.Where(s => s.UserID == (int)Session["userid"]);

            if (tmp == null) return View("~/Views/Home/Payment.cshtml");
            else return View(film);
        }
        // GET: Films/Details/5
        public ActionResult Theater(int? id)
        {
            var history = db.Histories;
            var tmp = history.Where(s => s.UserID == (int)Session["userid"]);
            
            if (tmp == null) return View("~/Views/Home/Payment.cshtml");
            var comments = db.Comments.Include(c => c.Film).Include(c => c.User);
            ViewBag.comments = comments.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Film film = db.Films.Find(id);
            if (film == null)
            {
                return HttpNotFound();
            }
            if (tmp == null) return View("~/Views/Home/Payment.cshtml");
            else return View(film);
        }
        [HttpPost]
        public ActionResult Theater(int? id,int UserID,int FilmID,string Des)
        {
            int CommentsID = 1;
            var list = db.Comments;

            for (int i = 1; i <= list.ToList().Count + 1; i++)
            {
                Comment user = db.Comments.Find(i);
                if (user == null) CommentsID = i;
            }
            Comment comment = new Comment();
            comment.CommentID = CommentsID;
            comment.Des = Des;
            comment.UserID = UserID;
            comment.FilmID = FilmID;
            comment.TimeStamp = DateTime.Now;
            db.Comments.Add(comment);
            db.SaveChanges();

            var comments = db.Comments.Include(c => c.Film).Include(c => c.User);
            ViewBag.comments = comments.ToList();

            
            Film film = db.Films.Find(FilmID);
            if (film == null)
            {
                return HttpNotFound();
            }
            var history = db.Histories;
            var tmp = history.Where(s => s.UserID == (int)Session["userid"]);

            if (tmp == null) return View("~/Views/Home/Payment.cshtml");
            else 
            return View("Theater", film);
        }
        [HttpPost]
        public ActionResult Rating(int? id, int FilmID, int Rating)
        {
            var comments = db.Comments.Include(c => c.Film).Include(c => c.User);
            ViewBag.comments = comments.ToList();
            Film film = new Film();
            film = db.Films.Find(FilmID);
            if (Rating>10||Rating<0) return View("Theater", film);
            if (film.Rating == null) film.Rating = 10;
            film.Rating = (film.Rating + Rating)/2;
            db.Entry(film).State = EntityState.Modified;
            db.SaveChanges();
            return View("Theater",film);
        }
        public ActionResult Dashboard()
        {
            var histories = from c in db.Histories select c;
            ViewBag.history = histories.ToList();
            Double sotien = 0;
                
            int numofpeo = 1;
            var list = histories.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                sotien += (double)list[i].Money;
            }
            var users = from d in db.Users select d;
            var listt = users.ToArray();
            int dem = 0;
            for (int i = 1; i <= listt.Length; i++)
            {
                var tmp = db.Histories.FirstOrDefault(x => x.UserID == i);
                if (tmp != null) dem++;
            }
            Session["Gain"] = sotien;
            Session["NumBuy"] = dem;
            Session["Numdont"] = listt.Length - dem;
            Session["Percent"] = (float)dem / listt.Length * 100;
            if (Session["username"] != "" && Session["username"] != null) return View();
            return View("~/Views/Login/Index.cshtml");
        }
        public ActionResult Payment()
        {
            if (Session["username"] != "" && Session["username"] != null) return View(db.Packets.ToList());
            return View("~/Views/Login/Index.cshtml");
        }
        public ActionResult Payment2()
        {
            int userid = int.Parse(Session["userid"].ToString());
            var vouchers = db.UserVouchers.Where(s=>s.UserID==userid).Include(u => u.User).Include(u => u.Voucher);
            ViewBag.voucher = vouchers.ToList();
            ViewBag.voucherID = new SelectList(db.Vouchers, "VoucherID", "Num_Discount");
            if (Session["username"] != "" && Session["username"] != null) return View(db.Packets.ToList());
            return View("~/Views/Login/Index.cshtml");
        }
        [HttpPost]
        public ActionResult ToPays(int Package = 0, int Month = 0, int voucherid = 0)
        {
            int userid = int.Parse(Session["userid"].ToString());
            if (voucherid == 0) voucherid = 2;
            else { 
                var v= db.UserVouchers.FirstOrDefault(x => x.UserID == userid && x.VoucherID==voucherid);
                v.Status = false;
                db.Entry(v).State= EntityState.Modified;
                db.SaveChanges();
            }
            // Nhận giohang từ View truyền sang
            History history = new History();
            var packet = db.Packets.FirstOrDefault(x => x.PacketId == Package.ToString());
            float money = 10000;
            money = (float)packet.Price;
           
            int historyid = 1;
            while (true)
            {
                var tmp = db.Histories.FirstOrDefault(x => x.HistoryID == historyid);
                if (tmp == null) break;
                historyid++;
            }
            history.PacketId = Package.ToString();
            history.HistoryID = historyid;
            history.Money = Month * money;
            history.TimeStamp = DateTime.Now;
            history.UserID = int.Parse(Session["userid"].ToString());
            history.VoucherID = voucherid;
            if (voucherid != -1)
            {
                Voucher voucher = db.Vouchers.FirstOrDefault(x => x.VoucherID == voucherid);
                if (voucher.Amount > 0)
                {
                    // history.VoucherID = voucherid;
                    history.Money -= voucher.Num_Discount;
                    
                }
                //Voucher voucher = db.Vouchers.FirstOrDefault(x => x.VoucherID == voucherid);
            }
            //history.VoucherID = voucherid;
            db.Histories.Add(history);
            db.SaveChanges();
            if (Session["role"].ToString().Equals("true") == true)
                return RedirectToAction("Dashboard", "Home");
            else return RedirectToAction("Index", "Home");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            if (Session["username"] != "" && Session["username"] != null) return View();
            return View("~/Views/Login/Index.cshtml");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            if (Session["username"] != "" && Session["username"] != null) return View();
            return View("~/Views/Login/Index.cshtml");
        }
        public ActionResult IndexCategory()
        {
            return View(db.Categories.ToList());
        }
        public ActionResult IndexGenre()
        {
            return View(db.Genres.ToList());
        }
        public ActionResult DetailsUser(int? id)
        {
            var histories = from c in db.Histories select c;
            ViewBag.history = histories.Where(s=>s.UserID==id).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Username,Password,Fullname,Email,Phone,Address,Status")] User user, string password)
        {
             
            user.Username= Session["username"].ToString();
            user.Status = (bool)Session["role"];
            string s = password;
            if (ModelState.IsValid && s.CompareTo(user.Password) == 0)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (s.CompareTo(user.Password) != 0) ViewBag.fail = "Nhập lại mật khẩu sai";
            return View(user);
        }

    }
}