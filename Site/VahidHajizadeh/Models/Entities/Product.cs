using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models
{
    public class Product : BaseEntity
    {
        public Product()
        {
            //Orders=new List<Order>();
            OrderDetails = new List<OrderDetail>();
            ProductDiscounts = new List<ProductDiscount>();
        }
        [Display(Name = "Code", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Code { get; set; }
 

        [Display(Name = "Title", ResourceType = typeof(Resources.Models.Product))]
        [StringLength(250, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public string Title { get; set; }

        [Display(Name = "پارامتر وب سایت")]
        public string UrlParam { get; set; }

        [Display(Name = "IsFree", ResourceType = typeof(Resources.Models.Product))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsFree { get; set; }
         
        [Display(Name = "FileUrl", ResourceType = typeof(Resources.Models.Product))]
        public string FileUrl { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Product))]
        [Column(TypeName = "Money")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal Amount { get; set; }

        [Display(Name="توضیحات کوتاه")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Summery { get; set; }

        [Display(Name = "Body", ResourceType = typeof(Resources.Models.Product))]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }

        [Display(Name = "ImageUrl", ResourceType = typeof(Resources.Models.Product))]
        public string ImageUrl { get; set; }
   

        [Display(Name = "ProductGroupId", ResourceType = typeof(Resources.Models.Product))]
        public Guid? ProductGroupId { get; set; }
         
        public virtual ProductGroup ProductGroup { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; }

       
        [Display(Name = "قیمت بعد از تخفیف")]
        public decimal? DiscountAmount { get; set; }

        [Display(Name = "در حراج است؟")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInPromotion { get; set; }

        [Display(Name = "در صفحه اصلی سایت است؟")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInHome { get; set; }

        [Display(Name = "تعداد دریافت")]
        public int BuyCount { get; set; }

        [NotMapped]
        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Order))]
        public string AmountStr
        {
            get { return Amount.ToString("N0"); }
        }


        [NotMapped]
        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Order))]
        public string DiscountAmountStr
        {
            get
            {
                if(DiscountAmount==null)
                     return Amount.ToString("N0");
                return DiscountAmount.Value.ToString("N0");
            }
        }

        [Display(Name = "محصول فیزیکی است")]
        public bool IsPhysicalProduct { get; set; }

    }
}
