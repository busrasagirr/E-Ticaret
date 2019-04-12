using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_Ticaret.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace E_Ticaret.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UrunlersController : Controller
    {
        private ETICARETEntities db = new ETICARETEntities();

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public void menuAyar()
        {
            if (isAdminUser())
            {
                ViewBag.display = "block";
            }
            else
            {
                ViewBag.display = "None";
            }
        }

        // GET: Urunlers
        public ActionResult Index()
        {
            var urunlers = db.Urunlers.Include(u => u.Kategori);
            return View(urunlers.ToList());
        }

        // GET: Urunlers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunlers.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }

        // GET: Urunlers/Create
        public ActionResult Create()
        {
            ViewBag.RefKatID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi");
            return View();
        }

        // POST: Urunlers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UrunID,UrunAdi,UrunAciklamasi,UrunFiyati,RefKatID")] Urunler urunler, HttpPostedFileBase urunresim)
        {
            if (ModelState.IsValid)
            {
                db.Urunlers.Add(urunler);
                db.SaveChanges();
                if (urunresim != null && urunresim.ContentLength > 0)
                {
                    string dosyaadi = Path.Combine(Server.MapPath("~/images"), urunler.UrunID + ".jpg");
                    urunresim.SaveAs(dosyaadi);
                }
                return RedirectToAction("Index");
            }
            ViewBag.RefKatID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", urunler.RefKatID);
            return View(urunler);
        }

        // GET: Urunlers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunlers.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            ViewBag.RefKatID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", urunler.RefKatID);
            return View(urunler);
        }

        // POST: Urunlers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UrunID,UrunAdi,UrunAciklamasi,UrunFiyati,RefKatID")] Urunler urunler, HttpPostedFileBase urunresim)
        {
            if (ModelState.IsValid)
            {
                db.Entry(urunler).State = EntityState.Modified;
                db.SaveChanges();
                if (urunresim != null && urunresim.ContentLength > 0)
                {
                    string dosyaadi = Path.Combine(Server.MapPath("~/images"), urunler.UrunID + ".jpg");
                    urunresim.SaveAs(dosyaadi);
                }
                return RedirectToAction("Index");
            }
            ViewBag.RefKatID = new SelectList(db.Kategoris, "KategoriID", "KategoriAdi", urunler.RefKatID);
            return View(urunler);
        }

        // GET: Urunlers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunlers.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }

        // POST: Urunlers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Urunler urunler = db.Urunlers.Find(id);
            db.Urunlers.Remove(urunler);
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
