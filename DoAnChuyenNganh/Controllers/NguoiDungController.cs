using DoAnChuyenNganh.Models;
using Facebook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }
        DataPetBestDataContext data = new DataPetBestDataContext();
        [HttpPost]
        public ActionResult Register(FormCollection collection, ThanhVien tv)
        {
            var TK = collection["TaiKhoan"]; //
            var MK = collection["Password"];
            var XMK = collection["XNMK"];
            var hoten = collection["HoTen"];
            var DiaChi = collection["Diachi"];
            var email = collection["email"];
            var sdt = collection["sdt"];
            //Save về Database

            tv.TaiKhoan = TK;
            tv.MatKhau = XMK;
            tv.HoTen = hoten;
            tv.DiaChi = DiaChi;
            tv.Email = email;
            tv.SDT = sdt;


            data.ThanhViens.InsertOnSubmit(tv);
            data.SubmitChanges();
            return RedirectToAction("DangKy", "NguoiDung");

        }


        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var TK = collection["username"];
            var MK = collection["password"];

            var tv = data.ThanhViens.SingleOrDefault(k => k.TaiKhoan == TK && k.MatKhau == MK);
            if (tv != null)
            {
                Session["tk"] = tv;
                Session["tendangnhap"] = tv.HoTen;
                Session["matv"] = tv.MaTV;
                return RedirectToAction("Index", "SalePet");
            }
            else
            {
                ViewBag.Thongbao = "Tài khoản không hợp lệ";
            }

            return View();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        public ActionResult SignOut()

        {
            Session.Clear();
            return RedirectToAction("Index", "SalePet");
        }
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppID"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecrect"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        /***********************************************/
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppID"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecrect"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            if (result == null)
            {
                return View("LoginFailed");
            }
            var accessToken = result.access_token;
            fb.AccessToken = accessToken;
            if (!string.IsNullOrEmpty(accessToken))
            {
                dynamic me = fb.Get("me?fields = fisrt_name, middle_name, last_name, id");
                string username = me.name;

                string TaiKhoan = me.id;

               

                var user = new ThanhVien();
                user.HoTen = username;
                user.TaiKhoan = TaiKhoan;
                user.MatKhau = "Nhi2282000";
                user.Email = "nhitruong@gmail.com";
                user.DiaChi = "Trường Mầm Non hoa cỏ mùa xuân";
                user.SDT = "0568555100";

                
                var kq = InsertForFacebook(user);
                if (kq > 0)
                {
                    var userSession = new ThanhVien();
                    userSession.TaiKhoan = user.HoTen;
                    userSession.MaTV = kq;
                    Session["matv"] = userSession.MaTV;
                    Session["tk"] = userSession;
                    Session.Add("tendangnhap", userSession.TaiKhoan);
                }
            }
            return RedirectToAction("Index", "SalePet");
        }

        public int InsertForFacebook(ThanhVien entity)
        {
            var user = data.ThanhViens.SingleOrDefault(x => x.HoTen == entity.HoTen);
            if (user == null)
            {
                data.ThanhViens.InsertOnSubmit(entity);
                data.SubmitChanges();
                return entity.MaTV;
            }
            else
            {
                return user.MaTV;
            }
        }

        //-----------
        public ActionResult ThongTinChiTiet(int id)
        {
            var info = from IC in data.ThanhViens where IC.MaTV == id select IC;
            return View(info.Single());
        }
        [HttpPost, ActionName("ThongTinChiTiet")]
        public ActionResult SuaThongTinChiTiet(int id)
        {
            ThanhVien Cus = data.ThanhViens.SingleOrDefault(n => n.MaTV == id);
            UpdateModel(Cus);
            data.SubmitChanges();
            return RedirectToAction("ThongTinChiTiet", "NguoiDung");
        }
        //====
    }
}