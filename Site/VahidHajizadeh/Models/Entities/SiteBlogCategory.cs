using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SiteBlogCategory : BaseEntity
    {
        public SiteBlogCategory()
        {
            SiteBlogs = new List<SiteBlog>();
        } 

        [Display(Name="عنوان گروه")]
        public string Title { get; set; }

        [Display(Name="اولویت نمایش")]
        public int Order { get; set; }

        [Display(Name="پارامتر صفحه")]
        public string UrlParam { get; set; }

        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }

        public virtual ICollection<SiteBlog> SiteBlogs { get; set; }
    }
}
