using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductListViewModel : _BaseViewModel
    {
        public ProductGroup ProductGroup { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> SideBarProducts { get; set; }
        public List<ProductGroupItemViewModel> SideBarProductGroups { get; set; }
        public List<SiteBlog> SideBarBlogs { get; set; }
    }

    public class ProductGroupItemViewModel
    {
        public ProductGroup ProductGroup { get; set; }
        public int Quantity { get; set; }
    }
}