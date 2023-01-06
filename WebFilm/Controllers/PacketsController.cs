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
    public class PacketsController : Controller
    {
        private WebXemPhimEntities db = new WebXemPhimEntities();

        // GET: Packets
        public ActionResult Index()
        {
            return View(db.Packets.ToList());
        }

        // GET: Packets/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Packet packet = db.Packets.Find(id);
            if (packet == null)
            {
                return HttpNotFound();
            }
            return View(packet);
        }

        // GET: Packets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Packets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PacketId,PacketName,Price,Decription")] Packet packet)
        {
            if (ModelState.IsValid)
            {
                db.Packets.Add(packet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(packet);
        }

        // GET: Packets/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Packet packet = db.Packets.Find(id);
            if (packet == null)
            {
                return HttpNotFound();
            }
            return View(packet);
        }

        // POST: Packets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PacketId,PacketName,Price,Decription")] Packet packet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(packet);
        }

        // GET: Packets/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Packet packet = db.Packets.Find(id);
            if (packet == null)
            {
                return HttpNotFound();
            }
            return View(packet);
        }

        // POST: Packets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Packet packet = db.Packets.Find(id);
            db.Packets.Remove(packet);
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
