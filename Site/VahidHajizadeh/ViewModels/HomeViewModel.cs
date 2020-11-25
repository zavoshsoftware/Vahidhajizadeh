using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class HomeViewModel : _BaseViewModel
    {
        public List<Product> HomeProduct { get; set; }
        public List<SiteBlog> Blogs { get; set; }
        public List<SiteBlogCategory> SiteBlogCategories { get; set; }
        public List<Slider> Sliders { get; set; }
        public List<Video> Videos { get; set; }
    }
}  