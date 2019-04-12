using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_Ticaret.Models;

namespace E_Ticaret.Controllers
{
    public class SiparisDetaysController : Controller
    {
        private ETICARETEntities db = new ETICARETEntities();

        // GET: SiparisDetays
        public ActionResult Index()
        {
            var siparisDetays = db.SiparisDetays.Include(s => s.Sepet).Include(s => s.Sipari).Include(s => s.Urunler);
            return View(siparisDetays.ToList());
        }

        // GET: SiparisDetays/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiparisDetay siparisDetay = db.SiparisDetays.Find(id);
            if (siparisDetay == null)
            {
                return HttpNotFound();
            }
            return View(siparisDetay);
        }

        // GET: SiparisDetays/Create
        public ActionResult Create()
        {
            ViewBag.RefSepetID = new SelectList(db.Sepets, "SepetID", "RefKulID");
            ViewBag.SiparisDetayID = new SelectList(db.Siparis, "SiparisID", "RefKulID");
            ViewBag.RefUrunID = new SelectList(db.Urunlers, "UrunID", "UrunAdi");
            return View();
        }

        // POST: SiparisDetays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SiparisDetayID,RefSiparisID,RefUrunID,RefSepetID,Kargo,TeslimEdildi")] SiparisDetay siparisDetay)
        {
            if (ModelState.IsValid)
            {
                db.SiparisDetays.Add(siparisDetay);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RefSepetID = new SelectList(db.Sepets, "SepetID", "RefKulID", siparisDetay.RefSepetID);
            ViewBag.SiparisDetayID = new SelectList(db.Siparis, "SiparisID", "RefKulID", siparisDetay.SiparisDetayID);
            ViewBag.RefUrunID = new SelectList(db.Urunlers, "UrunID", "UrunAdi", siparisDetay.RefUrunID);
            return View(siparisDetay);
        }

        // GET: SiparisDetays/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiparisDetay siparisDetay = db.SiparisDetays.Find(id);
            if (siparisDetay == null)
            {
                return HttpNotFound();
            }
            ViewBag.RefSepetID = new SelectList(db.Sepets, "SepetID", "RefKulID", siparisDetay.RefSepetID);
            ViewBag.SiparisDetayID = new SelectList(db.Siparis, "SiparisID", "RefKulID", siparisDetay.SiparisDetayID);
            ViewBag.RefUrunID = new SelectList(db.Urunlers, "UrunID", "UrunAdi", siparisDetay.RefUrunID);
            return View(siparisDetay);
        }

        // POST: SiparisDetays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SiparisDetayID,RefSiparisID,RefUrunID,RefSepetID,Kargo,TeslimEdildi")] SiparisDetay siparisDetay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siparisDetay).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RefSepetID = new SelectList(db.Sepets, "SepetID", "RefKulID", siparisDetay.RefSepetID);
            ViewBag.SiparisDetayID = new SelectList(db.Siparis, "SiparisID", "RefKulID", siparisDetay.SiparisDetayID);
            ViewBag.RefUrunID = new SelectList(db.Urunlers, "UrunID", "UrunAdi", siparisDetay.RefUrunID);
            return View(siparisDetay);
        }

        // GET: SiparisDetays/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiparisDetay siparisDetay = db.SiparisDetays.Find(id);
            if (siparisDetay == null)
            {
                return HttpNotFound();
            }
            return View(siparisDetay);
        }

        // POST: SiparisDetays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SiparisDetay siparisDetay = db.SiparisDetays.Find(id);
            db.SiparisDetays.Remove(siparisDetay);
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
