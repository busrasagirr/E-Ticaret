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

namespace E_Ticaret.Controllers
{
    public class SiparisController : Controller
    {
        private ETICARETEntities db = new ETICARETEntities();

        // GET: Siparis
        public ActionResult Index()
        {
            var siparis = db.Siparis.Include(s => s.AspNetUser).Include(s => s.AspNetUser1).Include(s => s.SiparisDetay);
            return View(siparis.ToList());
        }
        public ActionResult SiparisTamamla()
        {
            string userID = User.Identity.GetUserId();
            IEnumerable<Sepet> sepetUrunleri = db.Sepets.Where(a => a.RefKulID == userID).ToList();

            string ClientId = "100300000"; // Bankadan aldığınız mağaza kodu
            string Amount = sepetUrunleri.Sum(a => a.Toplam).ToString(); // sepettteki ürünlerin toplam fiyatı
            string Oid = String.Format("{0:yyyyMMddHHmmss}", DateTime.Now); // sipariş id oluşturuyoruz. her sipariş için farklı olmak zorunda
            string OnayURL = "http://localhost:51552/Siparis/Tamamlandi"; // Ödeme tamamlandığında bankadan verilerin geleceği url
            string HataURL = "http://localhost:51552/Siparis/Tamamlandi";// Ödeme hata verdiğinde bankadan gelen verilerin gideceği url
            string RDN = "asdf"; // hash karşılaştırması için eklenen rast gele dizedir
            string StoreKey = "123456"; // Güvenlik anahtarı bankanın sanal pos sayfasından alıyoruz


            string TransActionType = "Auth"; // bu bölüm sabit değişmiyor
            string Instalment = "";
            string HashStr = ClientId + Oid + Amount + OnayURL + HataURL + TransActionType + Instalment + RDN + StoreKey; // Hash oluşturmak için bankanın bizden istediği stringleri birleştiriyoruz

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] HashBytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(HashStr);
            byte[] InputBytes = sha.ComputeHash(HashBytes);
            string Hash = Convert.ToBase64String(InputBytes);

            ViewBag.ClientId = ClientId;
            ViewBag.Oid = Oid;
            ViewBag.okUrl = OnayURL;
            ViewBag.failUrl = HataURL;
            ViewBag.TransActionType = TransActionType;
            ViewBag.RDN = RDN;
            ViewBag.Hash = Hash;
            ViewBag.Amount = Amount;
            ViewBag.StoreType = "3d_pay_hosting"; // Ödeme modelimiz biz buna göre anlatıyoruz 
            ViewBag.Description = "";
            ViewBag.XID = "";
            ViewBag.Lang = "tr";
            ViewBag.EMail = "busrasagirr@hotmail.com";
            ViewBag.UserID = "busrasagirr"; // bu id yi bankanın sanala pos ekranında biz oluşturuyoruz.
            ViewBag.PostURL = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate";

            return View();
        }


        public ActionResult Tamamlandi()
        {
            string userID = User.Identity.GetUserId();
            Sipari siparis = new Sipari()
            {
                Ad = Request.Form.Get("Ad"),
                Soyad=Request.Form.Get("Soyad"),
                Adres= Request.Form.Get("Adres"),
                Telefon= Request.Form.Get("Telefon"),
                Tarih= DateTime.Now,
                TCKimlik=Request.Form.Get("TCKimlik"),
                RefKulID=userID
            };

            List<Sepet> sepettekiurunler = db.Sepets.Where(x => x.RefKulID == userID).ToList();

            foreach (Sepet item in sepettekiurunler)
            {
                SiparisDetay detay = new SiparisDetay();
                detay.RefUrunID = item.RefUrunID;
                detay.Adet = item.Adet;
                detay.ToplamTutar = item.Toplam;

                siparis.SiparisDetays.Add(detay);
                db.Sepets.Remove(item);
            };
            db.Siparis.Add(siparis);
            db.SaveChanges();
            return View();
        }

        public ActionResult Hatali()
        {
            ViewBag.Hata = Request.Form;

            return View();
        }


        // GET: Siparis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sipari sipari = db.Siparis.Find(id);
            if (sipari == null)
            {
                return HttpNotFound();
            }
            return View(sipari);
        }

        // GET: Siparis/Create
        public ActionResult Create()
        {
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.SiparisID = new SelectList(db.SiparisDetays, "SiparisDetayID", "Kargo");
            return View();
        }

        // POST: Siparis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SiparisID,RefKulID,Ad,Soyad,Adres,Telefon,TCKimlik,Tarih")] Sipari sipari)
        {
            if (ModelState.IsValid)
            {
                db.Siparis.Add(sipari);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", sipari.RefKulID);
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", sipari.RefKulID);
            ViewBag.SiparisID = new SelectList(db.SiparisDetays, "SiparisDetayID", "Kargo", sipari.SiparisID);
            return View(sipari);
        }

        // GET: Siparis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sipari sipari = db.Siparis.Find(id);
            if (sipari == null)
            {
                return HttpNotFound();
            }
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", sipari.RefKulID);
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", sipari.RefKulID);
            ViewBag.SiparisID = new SelectList(db.SiparisDetays, "SiparisDetayID", "Kargo", sipari.SiparisID);
            return View(sipari);
        }

        // POST: Siparis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SiparisID,RefKulID,Ad,Soyad,Adres,Telefon,TCKimlik,Tarih")] Sipari sipari)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sipari).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", sipari.RefKulID);
            ViewBag.RefKulID = new SelectList(db.AspNetUsers, "Id", "Email", sipari.RefKulID);
            ViewBag.SiparisID = new SelectList(db.SiparisDetays, "SiparisDetayID", "Kargo", sipari.SiparisID);
            return View(sipari);
        }

        // GET: Siparis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sipari sipari = db.Siparis.Find(id);
            if (sipari == null)
            {
                return HttpNotFound();
            }
            return View(sipari);
        }

        // POST: Siparis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sipari sipari = db.Siparis.Find(id);
            db.Siparis.Remove(sipari);
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
