using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Web.Mvc;

namespace Models
{
    public class SiteBlog : BaseEntity
    {
        public SiteBlog()
        {
            SiteBlogImages=new List<SiteBlogImage>();
        }

        [Display(Name="عنوان")]
        public string Title { get; set; }

        [Display(Name = "توضیحات کوتاه")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }

        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }
         

        [Display(Name="پارامتر صفحه")]
        public string UrlParam { get; set; }


        [Display(Name = "خلاصه")]
        [DataType(DataType.MultilineText)]
        public string Summery { get; set; }
        public virtual ICollection<SiteBlogImage> SiteBlogImages { get; set; }


        public Guid SiteBlogCategoryId { get; set; }
        public virtual SiteBlogCategory SiteBlogCategory { get; set; }



    }
}
