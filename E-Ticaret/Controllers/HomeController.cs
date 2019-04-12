using E_Ticaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Controllers
{
    public class HomeController : Controller
    {
        ETICARETEntities ctx = new ETICARETEntities();

        public ActionResult Index()
        {
            ViewBag.KategoriListesi = ctx.Kategoris.ToList();
            ViewBag.SonUrunler = ctx.Urunlers.OrderByDescending(a=>a.UrunID).Skip(0).Take(12).ToList();
            return View();
        }
        public ActionResult Kategori(int id)
        {
            ViewBag.KategoriListesi = ctx.Kategoris.ToList();
            ViewBag.kategori = ctx.Kategoris.Find(id);
            return View(ctx.Urunlers.Where(x => x.RefKatID == id).ToList());
        }
        public ActionResult UrunDetay(int id)
        {
            ViewBag.KategoriListesi = ctx.Kategoris.ToList();
            return View(ctx.Urunlers.Find(id));
        }
        
 
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}