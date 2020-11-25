using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class BlogListViewModel : _BaseViewModel
    {
        public List<SiteBlog> SiteBlogs { get; set; }
        public SiteBlogCategory SiteBlogCategory { get; set; }
        public List<Product> SideBarProducts { get; set; }
        public List<ProductGroupItemViewModel> SideBarProductGroups { get; set; }
        public List<SiteBlog> SideBarBlogs { get; set; }
    }
}