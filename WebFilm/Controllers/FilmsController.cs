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
    public class FilmsController : Controller
    {
        private WebXemPhimEntities db = new WebXemPhimEntities();

        // GET: Films
        public ActionResult Index()
        {
            var films = db.Films.Include(f => f.Author).Include(f => f.Category).Include(f => f.Country).Include(f => f.Genre);
            return View(films.ToList());
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
            return View(film);
        }

        // GET: Films/Create
        public ActionResult Create()
        {
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "Fullname");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Catename");
            ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Countryname");
            ViewBag.GenreID = new SelectList(db.Genres, "GenreID", "Genrename");
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FilmID,Filmname,AuthorID,CategoryID,GenreID,Length,ReleaseDay,Rating,CurrnentEpisodes,Episodes,CountryID,Status,FilmPath,Poster")] Film film)
        {
            film.CurrnentEpisodes = 1;
            film.Episodes = 1;
            if (ModelState.IsValid)
            {
                db.Films.Add(film);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "Fullname", film.AuthorID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Catename", film.CategoryID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Countryname", film.CountryID);
            ViewBag.GenreID = new SelectList(db.Genres, "GenreID", "Genrename", film.GenreID);
            return View(film);
        }

        // GET: Films/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "Fullname", film.AuthorID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Catename", film.CategoryID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Countryname", film.CountryID);
            ViewBag.GenreID = new SelectList(db.Genres, "GenreID", "Genrename", film.GenreID);
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FilmID,Filmname,AuthorID,CategoryID,GenreID,Length,ReleaseDay,Rating,CurrnentEpisodes,Episodes,CountryID,Status,FilmPath,Poster")] Film film)
        {
            film.CurrnentEpisodes = 1;
            film.Episodes = 1;
            if (ModelState.IsValid)
            {
                db.Entry(film).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "Fullname", film.AuthorID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Catename", film.CategoryID);
            ViewBag.CountryID = new SelectList(db.Countries, "CountryID", "Countryname", film.CountryID);
            ViewBag.GenreID = new SelectList(db.Genres, "GenreID", "Genrename", film.GenreID);
            return View(film);
        }

        // GET: Films/Delete/5
        public ActionResult Delete(int? id)
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
            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Film film = db.Films.Find(id);
            db.Films.Remove(film);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
