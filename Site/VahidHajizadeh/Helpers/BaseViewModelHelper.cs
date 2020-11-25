using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Models;
using ViewModels;

//using ViewModels;

namespace Helpers
{
    public static class BaseViewModelHelper
    {
        private static DatabaseContext db = new DatabaseContext();

        public static List<ProductGroup> GetMenuProductGroup()
        {
            List<ProductGroup> parentProductGroups = db.ProductGroups.Where(c => c.IsDeleted == false&&c.IsActive)
                .OrderBy(current => current.Code).ToList();

      
            return parentProductGroups;
        }
        public static List<SiteBlogCategory> GetMenuBlogGroup()
        {
            List<SiteBlogCategory> blogCategories = db.SiteBlogCategories.Where(c => c.IsDeleted == false&&c.IsActive)
                .OrderBy(current => current.Order).ToList();

      
            return blogCategories;
        }
         

        public static List<SiteBlog> GetFooterBlogs()
        {
            List<SiteBlog> blogs = db.SiteBlogs.Where(c => c.IsDeleted == false&&c.IsActive)
                .OrderByDescending(current => current.CreationDate).Take(3).ToList();

      
            return blogs;
        }
         
    }
}