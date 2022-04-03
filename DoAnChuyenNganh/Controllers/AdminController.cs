using DoAnChuyenNganh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace DoAnChuyenNganh.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else

                return View();
        }
        //=Đăng nhập=
        public ActionResult LogInAdmin()
        {
            return View();
        }
        DataPetBestDataContext data = new DataPetBestDataContext();
        [HttpPost]
        public ActionResult LogInAdmin(FormCollection collection)
        {
            var TK = collection["UserName"];
            var MK = collection["Password"];

            var tk = data.Admins.SingleOrDefault(k => k.TaiKhoan != TK);
            var mk = data.Admins.SingleOrDefault(k => k.MatKhau != MK);

            var Acc = data.Admins.SingleOrDefault(k => k.TaiKhoan == TK && k.MatKhau == MK);
            if (Acc != null)
            {
                Session["account"] = Acc;
                Session["tendangnhap"] = Acc.HoTen;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                if (tk != null)
                {
                    ViewBag.Taikhoan = "Tài khoản không hợp lệ";
                }
                else if (mk != null)
                {
                    ViewBag.MatKhau = "Mật khẩu không hợp lệ";
                }
            }
            return this.LogInAdmin();
        }
        //=End đăng nhập=
        //=Thương hiệu=
        public ActionResult ViewThuonghieu()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                return View(data.NhaSanXuats.OrderByDescending(n => n.MaNSX).ToList());
            }

        }
        [HttpPost]
        public ActionResult ThemTH(FormCollection collection, NhaSanXuat TH)
        {
            var ThongTin = collection["ThongTin"];
            var TenTH = collection["TenThuongHieu"];
            //Lưu Save
            TH.TenNSX = TenTH;
            TH.ThongTin = ThongTin;
            data.NhaSanXuats.InsertOnSubmit(TH);
            data.SubmitChanges();
            ViewBag.ThongBao = "Thêm thương hiệu mới thành công";
            return RedirectToAction("ViewThuonghieu", "Admin");
        }
        [HttpGet]
        public ActionResult SuaTH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var thuonghieu = from TH in data.NhaSanXuats where TH.MaNSX == id select TH;
                return View(thuonghieu.Single());
            }
        }
        [HttpPost, ActionName("SuaTH")]
        public ActionResult Sua(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                NhaSanXuat thuonghieu = data.NhaSanXuats.SingleOrDefault(n => n.MaNSX == id);
                UpdateModel(thuonghieu);
                data.SubmitChanges();
                return RedirectToAction("ViewThuonghieu", "Admin");
            }
        }
        [HttpGet]
        public ActionResult XoaTH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var Thuonghieu = from th in data.NhaSanXuats where th.MaNSX == id select th;
                return View(Thuonghieu.Single());
            }
        }
        [HttpPost, ActionName("XoaTH")]
        public ActionResult Xoa(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                NhaSanXuat thuoghieu = data.NhaSanXuats.SingleOrDefault(n => n.MaNSX == id);
                data.NhaSanXuats.DeleteOnSubmit(thuoghieu);
                data.SubmitChanges();
                return RedirectToAction("ViewThuonghieu", "Admin");
            }
        }
        //=End thương hiệu=
        public ActionResult Member()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else

                return View(data.ThanhViens.OrderByDescending(n => n.MaTV).ToList());
        }
        //--Load SP ---
        public ActionResult LoadSP(int? page, string keyword)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                int pagesize = 8;
                int pagenum = (page ?? 1);
                if (!string.IsNullOrEmpty(keyword))
                {
                    TempData["kwd"] = keyword;
                    List<SanPham> lstSanPham = data.SanPhams.Where(n => n.TenSP.ToLower().Contains(keyword.ToLower())).ToList();
                    return View(lstSanPham.OrderByDescending(n => n.MaSP).ToPagedList(pagenum, pagesize));
                }


                return View(data.SanPhams.OrderByDescending(n => n.MaSP).ToPagedList(pagenum, pagesize));
            }
        }
        [HttpGet]
        public ActionResult ThemSP()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {

                ViewBag.MaDanhMuc = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDanhMuc), "MaDanhMuc", "TenDanhMuc");
                ViewBag.MaNSX = new SelectList(data.NhaSanXuats.ToList().OrderBy(n => n.TenNSX), "MaNSX", "TenNSX");
                ViewBag.MaLoaiSP = new SelectList(data.LoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai");

                return View();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSP(SanPham sp, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload1,
            HttpPostedFileBase fileUpload2, HttpPostedFileBase fileUpload3)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var filename = Path.GetFileName(fileUpload.FileName);
                var filename1 = Path.GetFileName(fileUpload1.FileName);
                var filename2 = Path.GetFileName(fileUpload2.FileName);
                var filename3 = Path.GetFileName(fileUpload3.FileName);

                var path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                var path1 = Path.Combine(Server.MapPath("~/Content/images/"), filename1);
                var path2 = Path.Combine(Server.MapPath("~/Content/images/"), filename2);
                var path3 = Path.Combine(Server.MapPath("~/Content/images/"), filename3);

                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";
                }
                else if (System.IO.File.Exists(path1))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";

                }
                else if (System.IO.File.Exists(path2))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";

                }
                else if (System.IO.File.Exists(path3))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";

                }
                else
                    //úp hình lên server
                    fileUpload.SaveAs(path);
                fileUpload1.SaveAs(path1);
                fileUpload2.SaveAs(path2);
                fileUpload3.SaveAs(path3);

                sp.HinhAnh = filename;
                sp.Hinh1 = filename1;
                sp.Hinh2 = filename2;
                sp.Hinh3 = filename3;
                data.SanPhams.InsertOnSubmit(sp);
                data.SubmitChanges();
                return RedirectToAction("LoadSP", "Admin");
            }
        }
        [HttpGet]
        public ActionResult SuaSP(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
                ViewBag.MaDanhMuc = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDanhMuc), "MaDanhMuc", "TenDanhMuc", sanpham.MaDanhMuc);
                ViewBag.MaNSX = new SelectList(data.NhaSanXuats.ToList().OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sanpham.MaNSX);
                ViewBag.MaLoaiSP = new SelectList(data.LoaiSanPhams.ToList().OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sanpham.MaLoaiSP);

        

                return View(sanpham);
            }
        }
        [HttpPost, ActionName("SuaSP")]
        [ValidateInput(false)]
        public ActionResult Sua(int id, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload1,
            HttpPostedFileBase fileUpload2, HttpPostedFileBase fileUpload3)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var filename1 = Path.GetFileName(fileUpload1.FileName);
                    var filename2 = Path.GetFileName(fileUpload2.FileName);
                    var filename3 = Path.GetFileName(fileUpload3.FileName);

                    var path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                    var path1 = Path.Combine(Server.MapPath("~/Content/images/"), filename1);
                    var path2 = Path.Combine(Server.MapPath("~/Content/images/"), filename2);
                    var path3 = Path.Combine(Server.MapPath("~/Content/images/"), filename3);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";
                    }
                    else if (System.IO.File.Exists(path1))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";

                    }
                    else if (System.IO.File.Exists(path2))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";

                    }
                    else if (System.IO.File.Exists(path3))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";

                    }
                    else
                        //úp hình lên server
                    fileUpload.SaveAs(path);
                    fileUpload1.SaveAs(path1);
                    fileUpload2.SaveAs(path2);
                    fileUpload3.SaveAs(path3);

                    SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
                    sp.HinhAnh = filename;
                    sp.Hinh1 = filename1;
                    sp.Hinh2 = filename2;
                    sp.Hinh3 = filename3;

                    UpdateModel(sp);
                    data.SubmitChanges();
                }
                return RedirectToAction("LoadSP", "Admin");
            }
        }
        [HttpGet]
        public ActionResult XoaSP(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var sanpham = from sp in data.SanPhams where sp.MaSP == id select sp;
                return View(sanpham.Single());
            }
        }
        [HttpPost, ActionName("XoaSP")]
        public ActionResult Xoasp(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
                data.SanPhams.DeleteOnSubmit(sanpham);
                data.SubmitChanges();
                return RedirectToAction("LoadSP", "Admin");
            }
        }
        [HttpPost]
        public ActionResult TimKiem(string keyword)
        {
            if (Session["account"] == null)
            {
                return RedirectToAction("LogInAdmin", "Admin");
            }
            else
            {
                int pagesize = 8;
                int pagenum = 1;

                TempData["kwd"] = keyword;
                List<SanPham> lstSanPham = data.SanPhams.Where(n => n.TenSP.ToLower().Contains(keyword.ToLower())).ToList();
                return View("LoadSP", lstSanPham.OrderByDescending(n => n.MaSP).ToPagedList(pagenum, pagesize));
            }
        }
        //==End Sản phẩm==
        //- Don Dat Hang -
        public ActionResult ViewDDH(int? page, string keyword)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                int pagesize = 10;
                int pagenum = (page ?? 1);
                if (!string.IsNullOrEmpty(keyword))
                {
                    TempData["kwd"] = keyword;
                    List<DonDatHang> lstCus = data.DonDatHangs.Where(n => n.ThanhVien.HoTen.ToLower().Contains(keyword.ToLower())).ToList();
                    return View(lstCus.OrderByDescending(n => n.MaDDH).ToPagedList(pagenum, pagesize));
                }
                return View(data.DonDatHangs.OrderByDescending(n => n.MaDDH).ToPagedList(pagenum, pagesize));
            }
        }
        public ActionResult Detail(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var chitiet = from sp in data.CT_DDHs where sp.MaDDH == id select sp;

                return View(chitiet);
            }
        }
        [HttpGet]
        public ActionResult SuaDDH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                DonDatHang ddh = data.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);

                ViewBag.TinhTrangGiaoHang = new SelectList(data.TinhTrangGiaoHangs.ToList().OrderBy(n => n.MaTrangThai), "MaTrangThai", "TinhTrang", ddh.TinhTrangGiaoHang);
                return View(ddh);
            }
        }
        [HttpPost, ActionName("SuaDDH")]
        public ActionResult SuaDH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                DonDatHang ddh = data.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);

                UpdateModel(ddh);
                data.SubmitChanges();
            }
            return RedirectToAction("ViewDDH", "Admin");
        }
        //==Bài viết admin==
        public ActionResult News(int? page, string keyword)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                int pagesize = 10;
                int pagenum = (page ?? 1);
                if (!string.IsNullOrEmpty(keyword))
                {
                    TempData["kwd"] = keyword;
                    List<TinTuc> lstCus = data.TinTucs.Where(n => n.TieuDe.ToLower().Contains(keyword.ToLower())).ToList();
                    return View(lstCus.OrderByDescending(n => n.MaTinTuc).ToPagedList(pagenum, pagesize));
                }
                return View(data.TinTucs.OrderByDescending(n => n.MaTinTuc).ToPagedList(pagenum, pagesize));
            }
        }
        [HttpGet]
        public ActionResult ThemNews()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                ViewBag.MaDanhMuc = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDanhMuc), "MaDanhMuc", "TenDanhMuc");
                return View();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemNews(TinTuc BA, HttpPostedFileBase fileUpload)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var filename = Path.GetFileName(fileUpload.FileName);

                var path = Path.Combine(Server.MapPath("~/Content/images/"), filename);

                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";
                }
                //úp hình lên server
                fileUpload.SaveAs(path);
                BA.Anh = filename;
                data.TinTucs.InsertOnSubmit(BA);
                data.SubmitChanges();
                return RedirectToAction("News", "Admin");
            }

        }
        [HttpGet]
        public ActionResult SuaBA(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                TinTuc BVA = data.TinTucs.SingleOrDefault(n => n.MaTinTuc == id);
                ViewBag.MaDanhMuc = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDanhMuc), "MaDanhMuc", "TenDanhMuc", BVA.MaDanhMuc);
                return View(BVA);
            }
        }
        [HttpPost, ActionName("SuaBA")]
        [ValidateInput(false)]
        public ActionResult SuaAdmin(int id, HttpPostedFileBase fileUpload)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
            if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/images/"), filename);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";
                    }
                    else
                        //úp hình lên server
                        fileUpload.SaveAs(path);
                    TinTuc BVA = data.TinTucs.SingleOrDefault(n => n.MaTinTuc == id);
                    BVA.Anh = filename;
                    UpdateModel(BVA);
                    data.SubmitChanges();

                }
                return RedirectToAction("News", "Admin");
            }
        }
        [HttpGet]
        public ActionResult XoaBA(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var BA = from ba in data.TinTucs where ba.MaTinTuc == id select ba;
                return View(BA.Single());
            }
        }
        [HttpPost, ActionName("XoaBA")]
        public ActionResult Xoaba(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                TinTuc BA = data.TinTucs.SingleOrDefault(n => n.MaTinTuc == id);
                data.TinTucs.DeleteOnSubmit(BA);
                data.SubmitChanges();
                return RedirectToAction("News", "Admin");
            }
        }
        /*=End News=*/
        //-Loai SP-
        public ActionResult Category()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
                return View(data.LoaiSanPhams.OrderByDescending(n => n.MaLoaiSP).ToList());
        }
        public ActionResult ThemLoaiSP(FormCollection collection, LoaiSanPham lsp)
        {
            var loaisanpham = collection["LoaiSanPham"];

            lsp.TenLoai = loaisanpham;
            data.LoaiSanPhams.InsertOnSubmit(lsp);
            data.SubmitChanges();
            return RedirectToAction("Category", "Admin");

        }
        [HttpGet]
        public ActionResult SuaLSP(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var LoaiSanPham = from lsp in data.LoaiSanPhams where lsp.MaLoaiSP == id select lsp;
                return View(LoaiSanPham.Single());
            }
        }
        [HttpPost, ActionName("SuaLSP")]

        public ActionResult Suaa(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                LoaiSanPham lsp = data.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == id);
                UpdateModel(lsp);
                data.SubmitChanges();
            }
            return RedirectToAction("Category", "Admin");
        }
        [HttpGet]
        public ActionResult XoaLSP(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var LoaiSanPham = from lsp in data.LoaiSanPhams where lsp.MaLoaiSP == id select lsp;
                return View(LoaiSanPham.Single());
            }
        }
        [HttpPost, ActionName("XoaLSP")]
        public ActionResult Xoaa(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                LoaiSanPham lsp = data.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == id);
                data.LoaiSanPhams.DeleteOnSubmit(lsp);
                data.SubmitChanges();
            }
            return RedirectToAction("Category", "Admin");

        }
        //--DatLich--
        public ActionResult ViewLoaiDV()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                return View(data.LoaiDVs.OrderByDescending(n => n.MaLoaiDV).ToList());
            }

        }
        [HttpPost]
        public ActionResult ThemDV(FormCollection collection, LoaiDV DV)
        {
            var TenLoaiDV = collection["TenLoaiDV"];
            //Lưu Save
            DV.TenLoaiDV = TenLoaiDV;
            data.LoaiDVs.InsertOnSubmit(DV);
            data.SubmitChanges();
            ViewBag.ThongBao = "Thêm loại dịch vụ thành công";
            return RedirectToAction("ViewLoaiDV", "Admin");
        }
        [HttpGet]
        public ActionResult SuaLDV(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var loaidv = from DV in data.LoaiDVs where DV.MaLoaiDV == id select DV;
                return View(loaidv.Single());
            }
        }
        [HttpPost, ActionName("SuaLDV")]
        public ActionResult SuaDV(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                LoaiDV loaidv = data.LoaiDVs.SingleOrDefault(n => n.MaLoaiDV == id);
                UpdateModel(loaidv);
                data.SubmitChanges();
                return RedirectToAction("ViewLoaiDV", "Admin");
            }
        }
        [HttpGet]
        public ActionResult XoaLDV(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var loaidv = from dv in data.LoaiDVs where dv.MaLoaiDV == id select dv;
                return View(loaidv.Single());
            }
        }
        [HttpPost, ActionName("XoaLDV")]
        public ActionResult XoaDV(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                LoaiDV loaidv = data.LoaiDVs.SingleOrDefault(n => n.MaLoaiDV == id);
                data.LoaiDVs.DeleteOnSubmit(loaidv);
                data.SubmitChanges();
                return RedirectToAction("ViewLoaiDV", "Admin");
            }
        }
        //=End dat lich=
        public ActionResult ViewDatLich(int? page, string keyword)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                int pagesize = 10;
                int pagenum = (page ?? 1);
                if (!string.IsNullOrEmpty(keyword))
                {
                    TempData["kwd"] = keyword;
                    List<DatLichHen> datlich = data.DatLichHens.Where(n => n.ThanhVien.HoTen.ToLower().Contains(keyword.ToLower())).ToList();
                    return View(datlich.OrderByDescending(n => n.MaTV).ToPagedList(pagenum, pagesize));
                }
                return View(data.DatLichHens.OrderByDescending(n => n.MaDL).ToPagedList(pagenum, pagesize));
            }
        }
        public ActionResult DetailDL(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var chitiet = from dlh in data.DatLichHens where dlh.MaDL == id select dlh;

                return View(chitiet.Single());
            }
        }
        [HttpGet]
        public ActionResult SuaDL(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                DatLichHen dlh = data.DatLichHens.SingleOrDefault(n => n.MaDL == id);

                ViewBag.MaTrangThaiDL = new SelectList(data.TrangThaiDatLiches.ToList().OrderBy(n => n.MaTrangThaiDL), "MaTrangThaiDL", "TenTrangThaiDL", dlh.MaTrangThaiDL );
                return View(dlh);
            }
        }
        [HttpPost, ActionName("SuaDL")]
        public ActionResult SuaDLH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                DatLichHen dlh = data.DatLichHens.SingleOrDefault(n => n.MaDL == id);

                UpdateModel(dlh);
                data.SubmitChanges();
            }
            return RedirectToAction("ViewDatLich", "Admin");
        }
    }
}