using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnChuyenNganh.Models
{
    public class GioHang
    {
        DataPetBestDataContext data = new DataPetBestDataContext();
        public int iidSP { get; set; }
        public string sTenSP { get; set; }
        public string simage_1 { get; set; }
        public double dGiaBan { get; set; }
        public int iSoluong { get; set; }
        public double dThanhtien
        {
            get { return iSoluong * dGiaBan; }
        }

        public GioHang(int idSP)
        {
            iidSP = idSP;
            SanPham SP = data.SanPhams.Single(n => n.MaSP == iidSP);
            sTenSP = SP.TenSP;
            simage_1 = SP.HinhAnh;
            dGiaBan = double.Parse(SP.GiaBan.ToString());
            iSoluong = 1;
        }

    }
}