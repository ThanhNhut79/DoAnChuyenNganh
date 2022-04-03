using DoAnChuyenNganh.Models;
using MoMo;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Controllers
{
    public class SalePetController : Controller
    {
        // GET: SalePet
        private List<SanPham> ProductNew(int count)
        {
            return data.SanPhams.OrderByDescending(a => a.NgayCapNhap).Take(count).ToList();
        }

        public ActionResult Index()
        {
            var SPmoi = ProductNew(12);
            return View(SPmoi);
        }
        DataPetBestDataContext data = new DataPetBestDataContext();
        public ActionResult TatCaSanPham(int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var all = from a in data.SanPhams select a;
            return View(all.ToPagedList(pagenum, pagesize));
        }
        public ActionResult SanPhamTheoDanhMuc(int id, int? page) {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var theoDM = from sp in data.SanPhams where sp.MaDanhMuc == id select sp;
            return View(theoDM);
        }
        public ActionResult SanPhamTheoLoai(int id, int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var theoLM = from sp in data.SanPhams where sp.MaLoaiSP == id select sp;
            return View(theoLM.ToPagedList(pagenum, pagesize));
        }
        public ActionResult chitietSP(int id)
        {
            var CTSP = from CT in data.SanPhams where CT.MaSP == id select CT;
            return View(CTSP.Single());
        }
        [HttpPost]
        public ActionResult Search(string keyword, int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var all = data.SanPhams.Where(x => x.TenSP.Contains(keyword));
            return View(all.ToPagedList(pagenum, pagesize));
        }
        public ActionResult LichSuMH(int id)
        {
            if (Session["tk"] == null)
                return RedirectToAction("LogIn", "NguoiDung");
            else
            {
                var LS = from ls in data.CT_DDHs.OrderByDescending(n => n.MaDDH) where ls.DonDatHang.MaTV == id select ls;
                return View(LS);
            }
        }
        public ActionResult XacNhanDH()
        {
            var SPmoi = ProductNew(12);
            return View(SPmoi);
        }
        public ActionResult PriceFilter(double from, double to, string type, int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            ViewData["from"] = from;
            ViewData["to"] = to;
            if (!String.IsNullOrEmpty(type))
            {
                var all = data.SanPhams.Where(sp => sp.GiaBan >= from && sp.GiaBan <= to && sp.MaLoaiSP == int.Parse(type));
                return View("SanPhamTheoLoai", all.ToPagedList(pagenum, pagesize));
            }
            else
            {
                var all = data.SanPhams.Where(sp => sp.GiaBan >= from && sp.GiaBan <= to);
                return View("TatCaSanPham", all.ToPagedList(pagenum, pagesize));
            }
        }
        
    }
}