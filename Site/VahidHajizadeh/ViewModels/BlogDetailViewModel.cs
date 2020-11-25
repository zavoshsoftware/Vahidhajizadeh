using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class BlogDetailViewModel : _BaseViewModel
    {
        public SiteBlog SiteBlog { get; set; }

        public List<SiteBlog> RelatedBlogs { get; set; }
        public List<SiteBlogCategory> SideBarSiteBlogCategories { get; set; }

        public List<Product> SideBarProducts { get; set; }
        public List<ProductGroupItemViewModel> SideBarProductGroups { get; set; }
        public List<SiteBlog> SideBarBlogs { get; set; }

    }
}