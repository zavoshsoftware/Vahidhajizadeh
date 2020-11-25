using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels;

namespace VahidHajizadeh.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        public ActionResult Index()
        {
            HomeViewModel home = new HomeViewModel()
            {
                HomeProduct = db.Products.Where(c => c.IsDeleted == false && c.IsActive && c.IsInHome).OrderByDescending(c=>c.CreationDate).Take(8).ToList(),
                Blogs = db.SiteBlogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(4).ToList(),
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                Sliders = db.Sliders.Where(c => c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).Take(6).ToList(),
                SiteBlogCategories = db.SiteBlogCategories.Where(c => c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                Videos = db.Videos.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(4).ToList(),

            };
            return View(home);
        }
 
    }
}