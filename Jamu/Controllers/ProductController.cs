using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jamu.Models;
using System.IO;

namespace Jamu.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        public async Task<ActionResult> Index()
        {
            var products = db.Products.Include(p => p.Brand);
            return View(await products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductModel productModel = await db.Products.FindAsync(id);
            if (productModel == null)
            {
                return HttpNotFound();
            }
            return View(productModel);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name");
            ViewBag.Code = getCode();
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Code,Name,Price,Photos,BrandId,Expired,CreatedAt,UpdatedAt")] ProductModel productModel, 
            int[] CategoryId = null,
            HttpPostedFileBase upload = null
        )
        {
            if (ModelState.IsValid)
            {
                var Product = new ProductModel
                {
                    Code = productModel.Code,
                    Name = productModel.Name,
                    Price = productModel.Price,
                    BrandId = productModel.BrandId,
                    Expired  = productModel.Expired
                };

                if (upload != null && upload.ContentLength > 0)
                {
                    this.createDir("Uploads");
                    string extension = Path.GetExtension(upload.FileName).ToLower();
                    string fileName = createFileName() + "" + extension;
                    string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    upload.SaveAs(path);
                    Product.Image = fileName;

                }

                db.Products.Add(Product);

                if (CategoryId != null)
                {
                    foreach (var id in CategoryId)
                    {
                        var category = db.Categories.Find(id);
                        category.Product.Add(Product);

                    }

                }


                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name", productModel.BrandId);
            ViewBag.Code = getCode();
            return View(productModel);
        }

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductModel productModel = await db.Products.FindAsync(id);
            if (productModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name", productModel.BrandId);
            ViewBag.Categories = db.Categories.ToList();
            return View(productModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit
        (
            [Bind(Include = "Id,Code,Name,Price,Photos,BrandId,Expired,CreatedAt,UpdatedAt")] ProductModel productModel, 
            int[] CategoryId = null,
            HttpPostedFileBase upload = null
        )
        {
            if (ModelState.IsValid)
            {
                var Product = db.Products.Find(productModel.Id);
                Product.Code = productModel.Code;
                Product.Name = productModel.Name;
                Product.Price = productModel.Price;
                Product.BrandId = productModel.BrandId;
                Product.Expired = productModel.Expired;
                Product.UpdatedAt = DateTime.Now;

                if (upload != null && upload.ContentLength > 0)
                {
                    this.createDir("Uploads");
                    if (!String.IsNullOrWhiteSpace(Product.Image))
                    {
                        string pathExists = Path.Combine(Server.MapPath("~/Uploads"), Product.Image);
                        if (System.IO.File.Exists(pathExists))
                        {
                            System.IO.File.Delete(pathExists);
                        }
                    }

                    string extension = Path.GetExtension(upload.FileName).ToLower();
                    string fileName = createFileName() + "" + extension;
                    string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    upload.SaveAs(path);
                    Product.Image = fileName;
                }



                var currentSelected = db.Products.Include("Category")
                    .Single(x => x.Id == productModel.Id);
                currentSelected.Category.Clear();

                if (CategoryId != null)
                {
                    foreach (var id in CategoryId)
                    {
                        var category = db.Categories.Find(id);
                        category.Product.Add(Product);
                    }

                }

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name", productModel.BrandId);
            ViewBag.Categories = db.Categories.ToList();
            return View(productModel);
        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductModel productModel = await db.Products.FindAsync(id);
            if (productModel == null)
            {
                return HttpNotFound();
            }
            return View(productModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductModel productModel = await db.Products.FindAsync(id);

            if(!string.IsNullOrWhiteSpace(productModel.Image))
            {
                string path = Path.Combine(Server.MapPath("~/Uploads"), productModel.Image);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            db.Products.Remove(productModel);
            await db.SaveChangesAsync();
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

        private string getCode()
        {
            Random rnd = new Random();
            String timeTick = Convert.ToString(DateTime.Now.Ticks);
            return "P" + rnd.Next(1000, 1999) + "" + timeTick.Substring(0, 3);
        }

        private string createFileName()
        {
            return Convert.ToString(DateTime.Now.Ticks);
        }

        private void createDir(string foldername)
        {
            string path = Path.Combine(Server.MapPath("~/"), foldername);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
       

        }
    }
}
