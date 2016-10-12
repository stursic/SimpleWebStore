using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimpleWebStore.Models;
using System.IO;
using System.Text.RegularExpressions;
using PagedList;


namespace SimpleWebStore.Controllers
{
    public class ProductsController : Controller
    {
        private WebStoreDBEntities db = new WebStoreDBEntities();

        // GET: Products
        public ActionResult Index(string search, string tab, int? page)
        {
            if(tab == null)
            {
                ViewBag.Active = "all";
            }
            else
            {
                ViewBag.Active = tab;
            }


           
            var categories = db.Categories;
            var products = db.Products.Include(p => p.Categories);


            if (!String.IsNullOrEmpty(search))
            {
                ViewBag.Search = search;
                products = products.Where(s => s.Name.Contains(search)
                                       || s.Name.Contains(search));

                System.Diagnostics.Debug.WriteLine(products.Count()+ "First");
            }
            else
            {
                ViewBag.Search = "no";
            }


            

            BrowseProductsViewModel model = new BrowseProductsViewModel();
            model.Categories = categories.ToList();
            model.Products = products.OrderBy(a => a.Id).ToPagedList(1, 1);

            return View(model);
        }

        public PartialViewResult _productsPartialView(BrowseProductsViewModel model, string tab, int? page, string search)
        {


            int pageSize = 1;
            int pageNumber = (page ?? 1);

            if (tab.Equals("all"))
            {
                var products = db.Products.Include(p => p.Categories);
                model.Products = products.OrderBy(a => a.Id).ToPagedList(pageNumber, pageSize);

            }
            else
            {
                var products = db.Products.Include(p => p.Categories).Where(s => s.Categories.CategoryName.Equals(tab));
                model.Products = products.OrderBy(a => a.Id).ToPagedList(pageNumber, pageSize);

            }

            

            if (!search.Equals("no"))
            {
                var products = db.Products.Include(p => p.Categories).Where(s => s.Name.Contains(search));

                System.Diagnostics.Debug.WriteLine(products.Count());
                model.Products = products.OrderBy(a => a.Id).ToPagedList(pageNumber, pageSize);
            }

            var categories = db.Categories;
            model.Categories = categories;

            ViewBag.current = tab;
            
            return PartialView(model);
        }



        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoriesId = new SelectList(db.Categories, "Id", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Price,Description,CategoriesId")] Products products, HttpPostedFileBase productImg)
        {
            if (ModelState.IsValid)
            {

                if (productImg != null)
                {
                    var fileName = Path.GetFileName(productImg.FileName);
                    fileName = Regex.Replace(fileName, @"\s", "");
                    fileName = Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.Ticks + Path.GetExtension(fileName);
                    var directoryToSave = Server.MapPath(Url.Content("~/Content/Images"));


                    var pathToSave = Path.Combine(directoryToSave, fileName);
                    productImg.SaveAs(pathToSave);
                    products.Image = fileName;
                }

                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoriesId = new SelectList(db.Categories, "Id", "CategoryName", products.CategoriesId);
            return View(products);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoriesId = new SelectList(db.Categories, "Id", "CategoryName", products.CategoriesId);
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,Description,Image,CategoriesId")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoriesId = new SelectList(db.Categories, "Id", "CategoryName", products.CategoriesId);
            return View(products);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = db.Products.Find(id);
            db.Products.Remove(products);
            db.SaveChanges();
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
    }
}
