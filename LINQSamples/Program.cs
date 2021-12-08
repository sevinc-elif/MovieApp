using LINQSamples.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQSamples
{
    class CustomerModel
    {
        public CustomerModel()
        {
            this.Orders = new List<OrderModel>();
        }
        public string CustomerId{ get; set; }

        public string CustomerName { get; set; }

        public int OrderCount { get; set; }
        public List<OrderModel> Orders { get; set; }
    }
    class OrderModel
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public List<ProductModel> Products { get; set; }
    }
    class ProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            using (var db=new NorthwindContext())
            {
                
            }
            Console.ReadLine();
        }













        private static void TablolarTotal(NorthwindContext db)
        {
            //müşterilerin verdiği sipariş toplamı
            var customers = db.Customers
                .Where(cus => cus.CustomerId == "PERIC")
                .Select(cus => new CustomerModel
                {
                    CustomerId = cus.CustomerId,
                    CustomerName = cus.ContactName,
                    OrderCount = cus.Orders.Count,
                    Orders = cus.Orders.Select(order => new OrderModel
                    {
                        OrderId = order.OrderId,
                        Total = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice),
                        Products = order.OrderDetails.Select(od => new ProductModel
                        {
                            ProductId = od.ProductId,
                            Name = od.Product.ProductName,
                            Price = od.UnitPrice,
                            Quantity = od.Quantity
                        }).ToList()
                    }).ToList()
                })
                .OrderBy(i => i.OrderCount)
                .ToList();
            foreach (var item in customers)
            {
                Console.WriteLine(item.CustomerId + " " + item.CustomerName + " " + item.OrderCount);
                Console.WriteLine("siparişler");
                foreach (var order in item.Orders)
                {
                    Console.WriteLine("**************************************");
                    Console.WriteLine(order.OrderId + " " + order.Total);
                    foreach (var product in order.Products)
                    {
                        Console.WriteLine(product.ProductId + " " + product.Name + " " + product.Price + " " + product.Quantity);
                    }
                }
            }
        }

        private static void TablolarArası(NorthwindContext db)
        {
            //var products = db.Products.Include(p => p.Category).Where(p => p.Category.CategoryName == "Beverages").ToList();

            //var products = db.Products.Where(p => p.Category.CategoryName == "Beverages")
            //    .Select(p => new
            //    {
            //        name = p.ProductName,
            //        id = p.CategoryId,
            //        categoryname = p.Category.CategoryName
            //    }).ToList();
            //foreach (var item in products)
            //{
            //    Console.WriteLine(item.name+" "+item.id+" "+item.categoryname);
            //}

            //var categories = db.Categories.Where(p => p.Products.Count() == 0).ToList();
            //var categories = db.Categories.Where(p => p.Products.Any()).ToList();
            //foreach (var item in categories)
            //{
            //    Console.WriteLine(item.CategoryName);
            //}

            //left join
            //var products = db.Products
            //    .Select(p => new {
            //        companyName=p.Supplier.CompanyName,
            //        contactName=p.Supplier.ContactName,
            //        p.ProductName
            //    }).ToList();
            //foreach (var item in products)
            //{
            //    Console.WriteLine(item.ProductName+" "+item.companyName+" "+item.contactName);
            //}

            //query expressions
            //var products = (from p in db.Products
            //                where p.UnitPrice > 10
            //                select p).ToList();

            //inner join
            var products = (from p in db.Products
                            join s in db.Suppliers on p.SupplierId equals s.SupplierId
                            select new
                            {
                                p.ProductName,
                                contactName = s.ContactName,
                                companyName = s.CompanyName
                            }).ToList();
            foreach (var item in products)
            {
                Console.WriteLine(item.ProductName + " " + item.companyName + " " + item.contactName);
            }
        }

        private static void Silme(NorthwindContext db)
        {
            //var p = db.Products.Find(82);
            //if (p != null)
            //{
            //    db.Products.Remove(p);
            //    db.SaveChanges();
            //}

            var p = new Product() { ProductId = 80 };
            db.Products.Remove(p);
            //db.Entry(p).State = EntityState.Deleted;
            db.SaveChanges();
        }

        private static void Güncelleme2(NorthwindContext db)
        {
            var p = new Product() { ProductId = 1 };
            db.Products.Attach(p);
            p.UnitsInStock = 50;
            db.SaveChanges();

            //var product = db.Products.Find(1);
            //if (product != null)
            //{
            //    product.UnitPrice = 28;
            //    db.Update(product);
            //    db.SaveChanges();
            //}
        }

        private static void Güncelleme(NorthwindContext db)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == 1);
            if (product != null)
            {
                product.UnitsInStock += 10;
                db.SaveChanges();
                Console.WriteLine("veri güncellendi.");
            }
        }

        private static void Ekleme(NorthwindContext db)
        {
            var category = db.Categories.Where(i => i.CategoryName == "Beverages").FirstOrDefault();
            var p1 = new Product() { ProductName = "yeni ürün 5", Category = category };
            var p2 = new Product() { ProductName = "yeni ürün 6", Category = category };

            var products = new List<Product>()
                {
                    p1,p2
                };
            db.Products.AddRange(products);
            db.SaveChanges();

            Console.WriteLine("veriler eklendi");
            Console.WriteLine(p1.ProductId);
            Console.WriteLine(p2.ProductId);
        }

        private static void Ders4(NorthwindContext db)
        {
            //var result = db.Products.Count();
            //var result = db.Products.Count(i=>i.UnitPrice>10 && i.UnitPrice<30);
            //var result = db.Products.Count(i => !i.Discontinued);
            //var result = db.Products.Where(p=>p.CategoryId==2).Max(p=>p.UnitPrice);
            //var result = db.Products.Where(i => !i.Discontinued).Average(p=>p.UnitPrice);
            //Console.WriteLine(result);
            var result = db.Products.OrderBy(p => p.UnitPrice).ToList();
            foreach (var item in result)
            {
                Console.WriteLine(item.ProductName + " " + item.UnitPrice);
            }
        }

        private static void Ders3(NorthwindContext db)
        {
            //Tüm müşterilerin sadece contactName ve CustomerId kolonlarını getir
            //var customers = db.Customers.Select(p=>new { p.CustomerId,p.ContactName }).ToList();
            //foreach (var p in customers)
            //{
            //    Console.WriteLine(p.CustomerId + ' ' + p.ContactName);
            //}

            //Tüm müşteri kayıtları
            //var customers = db.Customers.ToList();
            //foreach (var p in customers)
            //{
            //    Console.WriteLine(p.ContactName);
            //}

            //Almanyada yaşayan müşteri isimleri
            //var customers = db.Customers.Select(p => new { p.Country, p.ContactName }).Where(p => p.Country == "Germany").ToList();
            //foreach (var p in customers)
            //{
            //    Console.WriteLine(  p.ContactName);
            //}

            //Diego Roel isimli müşteri nerede yaşamaktadır?
            //var customer = db.Customers.Where(p=>p.ContactName=="Diego Roel").FirstOrDefault();
            //Console.WriteLine(customer.Address);

            //Stokta olmayan ürünler
            //var products = db.Products.Select(i=>new { i.ProductName,i.UnitsInStock}).Where(p=>p.UnitsInStock==0).ToList();
            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductName);
            //}

            //Tüm çalışanların ad soyadlarını tek kolon halinde getirme
            //var employees = db.Employees.Select(i=> new { FullName=i.FirstName+" "+i.LastName}).ToList();
            //foreach (var e in employees)
            //{
            //    Console.WriteLine(e.FullName);
            //}

            //Ürünler tablosundaki ilk 5 kaydı alınız.
            var products = db.Products.Take(5).ToList();
            foreach (var p in products)
            {
                Console.WriteLine(p.ProductName);
            }
        }

        private static void Ders2(NorthwindContext db)
        {
            //var products = db.Products.Where(p => p.UnitPrice > 18).ToList();
            //var products = db.Products.Select(p=>new { p.ProductName,p.UnitPrice}).Where(p => p.UnitPrice > 18).ToList();
            //var products = db.Products.Where(p => p.UnitPrice > 18 && p.UnitPrice<30).ToList();
            //var products = db.Products.Where(p => p.CategoryId == 1).ToList();
            //var products = db.Products.Where(p => p.CategoryId == 1||p.CategoryId==5).ToList();
            //var products = db.Products.Where(p => p.CategoryId==1).Select(p=>new { p.ProductName,p.UnitPrice}).ToList();
            var products = db.Products.Where(i => i.ProductName.Contains("Ch")).ToList();
            foreach (var p in products)
            {
                Console.WriteLine(p.ProductName + ' ' + p.UnitPrice);
            }
        }

        private static void Ders1(NorthwindContext db)
        {
            //var products = db.Products.ToList();
            //var products = db.Products.Select(p =>new ProductModel{ Name=p.ProductName,Price=p.UnitPrice}).ToList();
            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.Name+' '+p.Price);
            //}
            //var product = db.Products.First();
            var product = db.Products.Select(p => new { p.ProductName, p.UnitPrice }).FirstOrDefault();
            Console.WriteLine(product.ProductName + ' ' + product.UnitPrice);
        }
    }
}
