using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class _BaseViewModel
    {
      public List<ProductGroup> MenuProductGroups { get; set; }
      public List<SiteBlogCategory> MenuBlogGroups { get; set; }
      public List<SiteBlog> footerBlogs { get; set; }
    }
}