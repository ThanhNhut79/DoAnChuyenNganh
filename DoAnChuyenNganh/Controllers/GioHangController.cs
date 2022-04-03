using DemoVNPay.Others;
using DoAnChuyenNganh.Models;
using MoMo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Controllers
{
    public class GioHangController : Controller
    {
        const string TRA_TIEN_KHI_NHAN_HANG = "1";
        // GET: GioHang
        public ActionResult Index()
        {

            return View();
        }
        public List<GioHang> Laygiohang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult Themgiohang(int id, string strURL)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang SP = lstGiohang.Find(n => n.iidSP == id);
            if (SP == null)
            {
                SP = new GioHang(id);
                lstGiohang.Add(SP);
                return Redirect(strURL);
            }
            else
            {
                SP.iSoluong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        public ActionResult GioHang()
        {
            List<GioHang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
                return RedirectToAction("Index", "SalePet");
            else
            {
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.Tongtien = TongTien();
                return View(lstGiohang);
            }
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongtien = TongTien();
            ViewBag.TongSoLuong = TongSoLuong();
            return PartialView();
        }
        public ActionResult Xoa1SP(int id)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iidSP == id);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iidSP == id);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "SalePet");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaAll()
        {
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "SalePet");
        }
        public ActionResult Capnhat(int id, FormCollection collection)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iidSP == id);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(collection["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult Dathang()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("DangNhap", "NguoiDung");
            if (Session["GioHang"] == null)
                return RedirectToAction("Index", "SalePet");
            else
            {
                ViewBag.Tongtien = TongTien();
                ViewBag.Tongsoluong = TongSoLuong();
                List<GioHang> lstGiohang = Laygiohang();
                return View(lstGiohang);
            }
        }
        public ActionResult ThanhToanMoMo()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("DangNhap", "NguoiDung");
            if (Session["GioHang"] == null)
                return RedirectToAction("Index", "SalePet");
            else
            {
                ViewBag.Tongtien = TongTien();
                ViewBag.Tongsoluong = TongSoLuong();
                List<GioHang> lstGiohang = Laygiohang();
                return View(lstGiohang);
            }
        }
        DataPetBestDataContext data = new DataPetBestDataContext();
        [HttpPost]
        public ActionResult DatHangBth()
        {
            DonDatHang ddh = new DonDatHang();
            ThanhVien Cus = (ThanhVien)Session["tk"];
            List<GioHang> gh = Laygiohang();

            ddh.NguoiNhan = Cus.HoTen;
            ddh.SDT = Cus.SDT;
            ddh.DiaChi = Cus.DiaChi;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Today.AddDays(4);

            ddh.MaTV = Cus.MaTV;
            ddh.DaThanhToan = "Chưa thanh toán";
            ddh.TinhTrangGiaoHang = int.Parse("1");
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Thêm chi tiết đơn hàng 
            foreach (var item in gh)
            {
                CT_DDH ctddh = new CT_DDH();
                ctddh.MaDDH = ddh.MaDDH;
                ctddh.MaSP = item.iidSP;
                ctddh.SoLuong = item.iSoluong;
                ctddh.DonGia = item.dThanhtien;
                data.CT_DDHs.InsertOnSubmit(ctddh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Index", "SalePet");

        }
        public ActionResult DatHangMoMo()
        {
            DonDatHang ddh = new DonDatHang();
            ThanhVien Cus = (ThanhVien)Session["tk"];
            List<GioHang> gh = Laygiohang();

            ddh.NguoiNhan = Cus.HoTen;
            ddh.SDT = Cus.SDT;
            ddh.DiaChi = Cus.DiaChi;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Today.AddDays(4);

            ddh.MaTV = Cus.MaTV;
            ddh.DaThanhToan = "Đã thanh toán";
            ddh.TinhTrangGiaoHang = int.Parse("1");
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Thêm chi tiết đơn hàng 
            foreach (var item in gh)
            {
                CT_DDH ctddh = new CT_DDH();
                ctddh.MaDDH = ddh.MaDDH;
                ctddh.MaSP = item.iidSP;
                ctddh.SoLuong = item.iSoluong;
                ctddh.DonGia = item.dThanhtien;
                data.CT_DDHs.InsertOnSubmit(ctddh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("ConfirmPaymentClient", "GioHang");
        }

        /*=Thanh Toán Bằng Ví Momo=*/
        public ActionResult Payment()
        {
            List<GioHang> gioHangs = Session["GioHang"] as List<GioHang>;
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOSWKP20211206";
            string accessKey = "EXvnJXExdlpSwkOf";
            string serectkey = "S8V7KQsf9hHjKBC4rk6vrcrRXhaxyKnH";
            string orderInfo = (string)Session["tendangnhap"];
            string returnUrl = "https://localhost:44365/GioHang/DatHangMoMo";
            string notifyurl = "http://ba1adf48beba.ngrok.io/GioHang/SavePayment";
            //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = gioHangs.Sum(n => n.dThanhtien).ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient()
        {
            //hiển thị thông báo cho người dùng
            return View();
        }

        [HttpPost]
        public void SavePayment()
        {
            //cập nhật dữ liệu vào db
        }
        /*Thanh toán momo*/

        /*=Thanh toán bằng VNPay=*/
        public ActionResult ThanhToanVNPay()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("DangNhap", "NguoiDung");
            if (Session["GioHang"] == null)
                return RedirectToAction("Index", "SalePet");
            else
            {
                ViewBag.Tongtien = TongTien();
                ViewBag.Tongsoluong = TongSoLuong();
                List<GioHang> lstGiohang = Laygiohang();
                return View(lstGiohang);
            }
        }
        /*==============================Thanh toán bằng VNPay===============================*/
        public ActionResult PaymentVNPay()
        {
            List<GioHang> gioHangs = Session["GioHang"] as List<GioHang>;
            string tien = gioHangs.Sum(n => n.dThanhtien * 100).ToString();
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.0.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", tien);
            pay.AddRequestData("vnp_BankCode", "");
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress());
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();


                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {

                        DonDatHang ddh = new DonDatHang();
                        ThanhVien Cus = (ThanhVien)Session["tk"];
                        List<GioHang> gh = Laygiohang();

                        ddh.NguoiNhan = Cus.HoTen;
                        ddh.SDT = Cus.SDT;
                        ddh.DiaChi = Cus.DiaChi;
                        ddh.NgayDat = DateTime.Now;
                        ddh.NgayGiao = DateTime.Today.AddDays(4);

                        ddh.MaTV = Cus.MaTV;
                        ddh.DaThanhToan = "Đã thanh toán";
                        ddh.TinhTrangGiaoHang = int.Parse("1");
                        data.DonDatHangs.InsertOnSubmit(ddh);
                        data.SubmitChanges();
                        //Thêm chi tiết đơn hàng 
                        foreach (var item in gh)
                        {
                            CT_DDH ctddh = new CT_DDH();
                            ctddh.MaDDH = ddh.MaDDH;
                            ctddh.MaSP = item.iidSP;
                            ctddh.SoLuong = item.iSoluong;
                            ctddh.DonGia = item.dThanhtien;
                            data.CT_DDHs.InsertOnSubmit(ctddh);
                        }
                        data.SubmitChanges();
                        Session["Giohang"] = null;
                        return RedirectToAction("ConfirmPaymentClient", "GioHang");

                    }
                    else
                    {

                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
        /*==================================================================================*/

    }
}