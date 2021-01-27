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
                Blogs = db.SiteBlogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(8).ToList(),
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                Sliders = db.Sliders.Where(c => c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).Take(6).ToList(),
                SiteBlogCategories = db.SiteBlogCategories.Where(c => c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList(),
                Videos = db.Videos.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(4).ToList(),

            };
            return View(home);
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

        [AllowAnonymous]
        [Route("contact")]
        public ActionResult Contact()
        {




            ContactViewModel result = new ContactViewModel()
            {
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
              
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
                SideBarProductGroups = GetSideBarProductGroups(),
                SideBarProducts = db.Products.Where(c => c.IsInHome && c.IsDeleted == false && c.IsActive).Take(3).ToList(),
                SideBarBlogs = db.SiteBlogs.Where(c => c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).Take(3).ToList()
            };
            return View(result);
        }

    }
}