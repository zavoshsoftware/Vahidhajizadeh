using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels;

namespace VahidHajizadeh.Controllers
{

    [Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.ProductGroup).Where(p => p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(products.ToList());
        }


        public ActionResult Create()
        {
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title");
            return View();
        }

        public string CalculateCount()
        {
            List<Product> products = db.Products.Where(c => c.IsDeleted == false).ToList();

            foreach (Product product in products)
            {
                int count = 0;

                List<OrderDetail> orderDetails = db.OrderDetails.Where(c => c.ProductId == product.Id).ToList();

                foreach (OrderDetail orderDetail in orderDetails)
                {
                    Order order = db.Orders.Find(orderDetail.OrderId);

                    if (order.IsPaid == true)
                    {
                        count++;
                    }
                }

                product.BuyCount = count;
            }

            db.SaveChanges();

            return "ok";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase fileupload, HttpPostedFileBase fileupload2)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    product.ImageUrl = newFilenameUrl;
                }
                #endregion

                #region Upload and resize image if needed
                if (fileupload2 != null)
                {
                    string filename = Path.GetFileName(fileupload2.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/Product/Files/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload2.SaveAs(physicalFilename);
                    product.FileUrl = newFilenameUrl;
                }
                #endregion

                product.BuyCount = 0;
                product.IsDeleted = false;
                product.CreationDate = DateTime.Now;
                product.Id = Guid.NewGuid();
                product.Code = CodeCreator.ReturnProductCode();
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title", product.ProductGroupId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title", product.ProductGroupId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase fileupload, HttpPostedFileBase fileupload2)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    product.ImageUrl = newFilenameUrl;
                }
                #endregion

                #region Upload and resize image if needed
                if (fileupload2 != null)
                {
                    string filename = Path.GetFileName(fileupload2.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/Product/Files/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload2.SaveAs(physicalFilename);
                    product.FileUrl = newFilenameUrl;
                }
                #endregion
                product.IsDeleted = false;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title", product.ProductGroupId);
            return View(product);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = db.Products.Find(id);
            product.IsDeleted = true;
            product.DeletionDate = DateTime.Now;

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



        [AllowAnonymous]
        [Route("product/{urlParam}")]
        public ActionResult Details(string urlParam)
        {

            Product product = db.Products.FirstOrDefault(c => c.UrlParam == urlParam);

            if (product == null)
            {
                return HttpNotFound();
            }
            ProductDetailViewModel productDetail = new ProductDetailViewModel()
            {
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                Product = product,
                SideBarProductGroups = GetSideBarProductGroups(),
                SideBarProducts = db.Products.Where(c => c.IsInHome && c.IsDeleted == false && c.IsActive).Take(3).ToList(),
                SideBarBlogs = db.SiteBlogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(3).ToList()

            };
            return View(productDetail);
        }

        [AllowAnonymous]
        [Route("category/{urlParam}")]
        public ActionResult List(string urlParam)
        {
            ProductGroup productGroup = db.ProductGroups.FirstOrDefault(c => c.UrlParam == urlParam);

            if (productGroup == null)
                return Redirect("/category");

            List<Product> products = db.Products.Where(c => c.ProductGroupId == productGroup.Id && c.IsDeleted == false && c.IsActive).ToList();


            ProductListViewModel productList = new ProductListViewModel()
            {
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                Products = products,
                ProductGroup = productGroup,
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                SideBarProductGroups = GetSideBarProductGroups(),
                SideBarProducts = db.Products.Where(c=>c.IsInHome&&c.IsDeleted==false&&c.IsActive).Take(3).ToList(),
                SideBarBlogs = db.SiteBlogs.Where(c=>c.IsDeleted==false&&c.IsActive).OrderByDescending(c=>c.CreationDate).Take(3).ToList()
            };
            return View(productList);
        }


        [AllowAnonymous]
        [Route("category")]
        public ActionResult PureList()
        {

            List<Product> products = db.Products.Where(c => c.IsDeleted == false && c.IsActive).ToList();


            ProductListViewModel productList = new ProductListViewModel()
            {
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                Products = products,
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                SideBarProductGroups = GetSideBarProductGroups(),
                SideBarProducts = db.Products.Where(c=>c.IsInHome&&c.IsDeleted==false&&c.IsActive).Take(3).ToList(),
                SideBarBlogs = db.SiteBlogs.Where(c=>c.IsDeleted==false&&c.IsActive).OrderByDescending(c=>c.CreationDate).Take(3).ToList()
            };
            return View(productList);
        }

        [AllowAnonymous]
        public List<ProductGroupItemViewModel> GetSideBarProductGroups()
        {
            List<ProductGroup> productGroups = db.ProductGroups.Where(c => c.IsDeleted == false && c.IsActive)
                .OrderBy(c => c.Code).ToList();

            List<ProductGroupItemViewModel> productGroupList = new List<ProductGroupItemViewModel>();

            foreach (ProductGroup productGroup in productGroups)
            {
                productGroupList.Add(new ProductGroupItemViewModel()
                {
                    ProductGroup = productGroup,
                    Quantity = db.Products.Count(c => c.ProductGroupId == productGroup.Id && c.IsDeleted == false && c.IsActive)
                });
            }

            return productGroupList;
        }
    }
}
