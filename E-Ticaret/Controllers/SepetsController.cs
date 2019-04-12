using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_Ticaret.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace E_Ticaret.Controllers
{
    public class SepetsController : Controller
    {
        private ETICARETEntities ctx = new ETICARETEntities();

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

        // GET: Sepets
        public ActionResult Index()
        {
            var sepets = ctx.Sepets.Include(s => s.AspNetUser).Include(s => s.Urunler);
            return View(sepets.ToList());
        }
        public ActionResult SepeteEkle(int? adet,int id)     //sepete ürün ekleme!  int -in yarındaki ? işareti boş değilse buraya gir demek!
        {
            string UserID = User.Identity.GetUserId();  //Giriş yapan kullanıcının  id -sini alıyor!

            Urunler urun = ctx.Urunlers.Find(id);
            Sepet sepettekiurunler = ctx.Sepets.FirstOrDefault(x => x.RefUrunID == id && x.RefKulID == UserID);
            if (sepettekiurunler == null)
            {
                Sepet yeniurun = new Sepet();
                yeniurun.RefKulID = UserID;
                yeniurun.RefUrunID = id;
                yeniurun.Toplam =(adet??1) * urun.UrunFiyati;
                yeniurun.Adet = adet??1;
                ctx.Sepets.Add(yeniurun);
            }
            else
            {
                sepettekiurunler.Adet = sepettekiurunler.Adet + (adet??1);
                sepettekiurunler.Toplam = sepettekiurunler.Adet * urun.UrunFiyati;
            }

            ctx.SaveChanges();
            
            return RedirectToAction("Index");
        }
        public ActionResult SepetGuncelle(int id,int? adet)
        {
            Sepet sepet = ctx.Sepets.Find(id);
            Urunler urun = ctx.Urunlers.Find(sepet.RefUrunID);

            sepet.Adet = adet ?? 1;
            sepet.Toplam = urun.UrunFiyati * sepet.Adet;
            ctx.SaveChanges();

            return RedirectToAction("Index");
        }

               

        // POST: Sepets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SepetID,RefKulID,RefUrunID,Adet,Toplam")] Sepet sepet)
        {
            if (ModelState.IsValid)
            {
                ctx.Entry(sepet).State = EntityState.Modified;
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RefKulID = new SelectList(ctx.AspNetUsers, "Id", "Email", sepet.RefKulID);
            ViewBag.RefUrunID = new SelectList(ctx.Urunlers, "UrunID", "UrunAdi", sepet.RefUrunID);
            return View(sepet);
        }

        // GET: Sepets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = ctx.Sepets.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }

            ctx.Sepets.Remove(sepet);
            ctx.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Sepets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sepet sepet = ctx.Sepets.Find(id);
            ctx.Sepets.Remove(sepet);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
