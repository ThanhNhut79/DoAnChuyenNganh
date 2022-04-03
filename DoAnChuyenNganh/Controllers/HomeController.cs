using DoAnChuyenNganh.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        DataPetBestDataContext data = new DataPetBestDataContext();
        public ActionResult NewsDM(int id, int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var news = from BA in data.TinTucs where BA.MaDanhMuc == id select BA;
            return View(news.ToPagedList(pagenum, pagesize));
        }
        public ActionResult Baiviet(int id)
        {
            var view = from v in data.TinTucs where v.MaTinTuc == id select v;
            return View(view.Single());
        }
        public ActionResult DatLich(int id,DatLichHen dlh)
        {
            if (Session["tk"] == null)
                return RedirectToAction("DangNhap", "NguoiDung");
            else
            {
                ViewBag.MaLoaiDV = new SelectList(data.LoaiDVs.ToList().OrderBy(n => n.TenLoaiDV), "MaLoaiDV", "TenLoaiDV", dlh.MaLoaiDV);
                var TV = from tv in data.ThanhViens where tv.MaTV == id select tv;
                return View(TV.Single());
            }
        }
        [HttpPost]
        public ActionResult DatLich(DatLichHen dlh) 
        {
            ThanhVien Cus = (ThanhVien)Session["tk"];
            dlh.MaTV = Cus.MaTV;
            dlh.HoTen = Cus.HoTen;
            dlh.SDT = Cus.SDT;
            dlh.DiaChi = Cus.DiaChi;
            dlh.MaTrangThaiDL = int.Parse("1");
            data.DatLichHens.InsertOnSubmit(dlh);
            data.SubmitChanges();
            return RedirectToAction("Index", "Home");
        }
        // Liên hệ gửi thư góp ý - tư vấn ================================   
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(FormCollection collection, LienHe LH)
        {
            var hoten = collection["name"];
            var sdt = collection["sdt"];
            var email = collection["email"];
            var noidung = collection["body"];

            LH.HoTen = hoten;
            LH.SDT = sdt;
            LH.EMAIL = email;
            LH.NoiDung = noidung;
            data.LienHes.InsertOnSubmit(LH);
            data.SubmitChanges();
            return RedirectToAction("Index", "Main");
        }

    }

}