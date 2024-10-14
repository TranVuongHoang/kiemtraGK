using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ProductDAL
    {
        private Model1 db;

        public ProductDAL()
        {
            db = new Model1();
        }

        public List<Sanpham> GetAllProducts()
        {
            return db.Sanpham.Include("LoaiSP").ToList();
        }

        public List<LoaiSP> GetAllCategories()
        {
            return db.LoaiSP.ToList();
        }

        public void AddProduct(Sanpham product)
        {
            db.Sanpham.Add(product);
            db.SaveChanges();
        }

        public void UpdateProduct(Sanpham product)
        {
            var existingProduct = db.Sanpham.SingleOrDefault(p => p.MaSP == product.MaSP);
            if (existingProduct != null)
            {
                existingProduct.TenSP = product.TenSP;
                existingProduct.NgayNhap = product.NgayNhap;
                existingProduct.MaLoai = product.MaLoai;
                db.SaveChanges();
            }
        }

        public void DeleteProduct(string maSP)
        {
            var product = db.Sanpham.SingleOrDefault(p => p.MaSP == maSP);
            if (product != null)
            {
                db.Sanpham.Remove(product);
                db.SaveChanges();
            }
        }

        public LoaiSP GetCategoryByName(string name)
        {
            return db.LoaiSP.FirstOrDefault(c => c.TenLoai == name);
        }
        public List<Sanpham> GetProductsByName(string tenSP)
        {
            return db.Sanpham.Include("LoaiSP")
                             .Where(p => p.TenSP.Contains(tenSP))
                             .ToList();
        }
    }
}
