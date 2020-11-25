using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class Video:BaseEntity
    {
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "تصویر")]
        public string ImageUrl { get; set; }

        [Display(Name = "لینک آپارات iframe")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string AparatLink { get; set; }
         
    }
}