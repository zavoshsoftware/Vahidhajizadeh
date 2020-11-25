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
    public class SiteBlogsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: SiteBlogs
        public ActionResult Index()
        {
            var siteBlogs = db.SiteBlogs.Include(s => s.SiteBlogCategory).Where(s => s.IsDeleted == false).OrderByDescending(s => s.CreationDate);
            return View(siteBlogs.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.SiteBlogCategoryId = new SelectList(db.SiteBlogCategories, "Id", "Title");
            return View();
        }

        // POST: SiteBlogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SiteBlog siteBlog, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    siteBlog.ImageUrl = newFilenameUrl;
                }
                #endregion

                siteBlog.IsDeleted = false;
                siteBlog.CreationDate = DateTime.Now;
                siteBlog.Id = Guid.NewGuid();
                db.SiteBlogs.Add(siteBlog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SiteBlogCategoryId = new SelectList(db.SiteBlogCategories, "Id", "Title", siteBlog.SiteBlogCategoryId);
            return View(siteBlog);
        }

        // GET: SiteBlogs/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteBlog siteBlog = db.SiteBlogs.Find(id);
            if (siteBlog == null)
            {
                return HttpNotFound();
            }
            ViewBag.SiteBlogCategoryId = new SelectList(db.SiteBlogCategories, "Id", "Title", siteBlog.SiteBlogCategoryId);
            return View(siteBlog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SiteBlog siteBlog, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    siteBlog.ImageUrl = newFilenameUrl;
                }
                #endregion
                siteBlog.IsDeleted = false;
                db.Entry(siteBlog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SiteBlogCategoryId = new SelectList(db.SiteBlogCategories, "Id", "Title", siteBlog.SiteBlogCategoryId);
            return View(siteBlog);
        }

        // GET: SiteBlogs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteBlog siteBlog = db.SiteBlogs.Find(id);
            if (siteBlog == null)
            {
                return HttpNotFound();
            }
            return View(siteBlog);
        }

        // POST: SiteBlogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            SiteBlog siteBlog = db.SiteBlogs.Find(id);
            siteBlog.IsDeleted = true;
            siteBlog.DeletionDate = DateTime.Now;

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
        [Route("blog")]
        [AllowAnonymous]
        public ActionResult PureList()
        {

            BlogListViewModel blogs = new BlogListViewModel()
            {
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                SiteBlogs = db.SiteBlogs.Where(current => current.IsDeleted == false && current.IsActive).OrderByDescending(c=>c.CreationDate).ToList(),
                SideBarProductGroups = GetSideBarProductGroups(),
                SideBarProducts = db.Products.Where(c => c.IsInHome && c.IsDeleted == false && c.IsActive).Take(3).ToList(),
                SideBarBlogs = db.SiteBlogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(3).ToList()
            };
            return View(blogs);
        }

        [Route("blog/category/{urlParam}")]
        [AllowAnonymous]
        public ActionResult List(string urlParam)
        {
            SiteBlogCategory siteBlogCategory = db.SiteBlogCategories.FirstOrDefault(c => c.UrlParam == urlParam);

            if (siteBlogCategory == null)
                return RedirectPermanent("/blog");

            BlogListViewModel blogs = new BlogListViewModel()
            {
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                SiteBlogs = db.SiteBlogs.Where(current => current.SiteBlogCategoryId == siteBlogCategory.Id && current.IsDeleted == false && current.IsActive).OrderByDescending(c => c.CreationDate).ToList(),
                SiteBlogCategory = siteBlogCategory,
                SideBarProductGroups = GetSideBarProductGroups(),
                SideBarProducts = db.Products.Where(c => c.IsInHome && c.IsDeleted == false && c.IsActive).Take(3).ToList(),
                SideBarBlogs = db.SiteBlogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(3).ToList()
            };
            return View(blogs);
        }
        [Route("blog/post/{urlParam}")]
        [AllowAnonymous]
        public ActionResult Details(string urlParam)
        {
            SiteBlog siteBlog = db.SiteBlogs.Where(current => current.UrlParam == urlParam)
                .Include(current => current.SiteBlogCategory).FirstOrDefault();

            if (siteBlog == null)
                return RedirectPermanent("/blog");


            BlogDetailViewModel blog = new BlogDetailViewModel()
            {
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                SiteBlog = siteBlog,
                SideBarProductGroups = GetSideBarProductGroups(),
                SideBarProducts =
                    db.Products.Where(c => c.IsInHome && c.IsDeleted == false && c.IsActive).Take(3).ToList(),
                SideBarBlogs = db.SiteBlogs.Where(c => c.IsDeleted == false && c.IsActive)
                    .OrderByDescending(c => c.CreationDate).Take(3).ToList(),
                RelatedBlogs =
                    db.SiteBlogs
                        .Where(c => c.SiteBlogCategoryId == siteBlog.SiteBlogCategoryId && c.IsDeleted == false &&
                                    c.IsActive && c.UrlParam != urlParam).OrderByDescending(c => c.CreationDate).Take(6)
                        .ToList()
            };

            return View(blog);
        }
    }
}
