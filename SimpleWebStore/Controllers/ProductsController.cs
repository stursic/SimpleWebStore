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

        //set page size to 5 products per page
        public const int pageSize = 5;
        //set path to images
        public const string ProductImagePath = "~/Content/Images/";


        [Route]
        // GET: Products
        public ActionResult Index(string search, string category, int? page)
        {

            //initialize the viewmodel
            BrowseProductsViewModel model = new BrowseProductsViewModel();

            //fill category
            if (category == null)
            {
                model.ActiveCategory = "All";
            }
            else
            {
                model.ActiveCategory = category;
            }

            //fill search
            if (!String.IsNullOrEmpty(search))
            {
                model.Search = search;
            }
            else
            {
                model.Search = "no";
            }

            //fill categories
            var categories = db.Categories;
            model.Categories = categories.ToList();
          
            return View(model);
        }

        public PartialViewResult _productsPartialView(BrowseProductsViewModel model, string category, int? page, string search)
        {

            int pageNumber = (page ?? 1);

            //fill products depending on category or search
            if (category.Equals("All"))
            {
                var products = db.Products.Include(p => p.Categories);
                model.Products = products.OrderBy(a => a.Id).ToPagedList(pageNumber, pageSize);

            }
            else
            {
                var products = db.Products.Include(p => p.Categories).Where(s => s.Categories.CategoryName.Equals(category));
                model.Products = products.OrderBy(a => a.Id).ToPagedList(pageNumber, pageSize);
            }
 
            if (!search.Equals("no"))
            {
                pageNumber = 1;
                var products = db.Products.Include(p => p.Categories).Where(s => s.Name.Contains(search));
                model.Products = products.OrderBy(a => a.Id).ToPagedList(pageNumber, pageSize);
            }

            //fill categories
            var categories = db.Categories;
            model.Categories = categories;

            //set active category
            model.ActiveCategory = category;
            //set search string
            model.Search = search;
            
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

                    //save image to folder Images and add current timestamp to name of the image
                    var fileName = Path.GetFileName(productImg.FileName);
                    fileName = Regex.Replace(fileName, @"\s", "");
                    fileName = Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.Ticks + Path.GetExtension(fileName);
                    var directoryToSave = Server.MapPath(Url.Content(ProductImagePath));

                    var pathToSave = Path.Combine(directoryToSave, fileName);
                    productImg.SaveAs(pathToSave);

                    //set image name to model to be saved to database
                    products.Image = fileName;
                }

                TempData["Feedback"] = String.Format("New product {0} sucessfully added!", "\"" + products.Name + "\"");

                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            TempData["Feedback"] = String.Format("Something went wrong with adding new product {0}!", "\"" + products.Name + "\"");

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
        public ActionResult Edit([Bind(Include = "Id,Name,Price,Description,Image,CategoriesId")] Products products,HttpPostedFileBase productImg)
        {
            if (ModelState.IsValid)
            {
                

                if (productImg != null)
                {
                    // if product image is being replaced, current image is deleted from Images folder
                    string fullPath = Request.MapPath(ProductImagePath + products.Image);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    // new image is saved to folder Images
                    var fileName = Path.GetFileName(productImg.FileName);
                    fileName = Regex.Replace(fileName, @"\s", "");
                    fileName = Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.Ticks + Path.GetExtension(fileName);
                    var directoryToSave = Server.MapPath(Url.Content(ProductImagePath));


                    var pathToSave = Path.Combine(directoryToSave, fileName);
                    productImg.SaveAs(pathToSave);

                    // filename of new image will be saved to database
                    products.Image = fileName;
                }


                TempData["Feedback"] = String.Format("Product {0} was sucessfully edited!", "\"" + products.Name + "\"");

                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            TempData["Feedback"] = String.Format("Something went wrong with editing product {0}!", "\"" + products.Name + "\"");
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

            string fullPath = Request.MapPath(ProductImagePath + products.Image);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            TempData["Feedback"] = String.Format("Product {0} was sucessfully deleted!", "\""+products.Name+"\"");

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
