﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order : BaseEntity
    {
        public Order()
        {
            ZarinpallAuthorities = new List<ZarinpallAuthority>();
            OrderDetails = new List<OrderDetail>();
        }
        [Display(Name = "Code", ResourceType = typeof(Resources.Models.Order))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Code { get; set; }
         
        [Display(Name = "UserId", ResourceType = typeof(Resources.Models.Order))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public Guid UserId { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Order))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal Amount { get; set; }

        [Display(Name = "TotalAmount", ResourceType = typeof(Resources.Models.Order))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "DiscountAmount", ResourceType = typeof(Resources.Models.Order))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal? DiscountAmount { get; set; }


        [Display(Name = "CityId", ResourceType = typeof(Resources.Models.Order))]
        public string City { get; set; }

        [Display(Name = "Address", ResourceType = typeof(Resources.Models.Order))]
        public string Address { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(Resources.Models.Order))]
        public string PostalCode { get; set; }
         
        [Display(Name = "Email", ResourceType = typeof(Resources.Models.Order))]
        public string Email { get; set; }
  public string CompanyName { get; set; }

 
        [Display(Name = "IsPaid", ResourceType = typeof(Resources.Models.Order))]
        public bool IsPaid { get; set; }

        [Display(Name = "PaymentDate", ResourceType = typeof(Resources.Models.Order))]
        public DateTime? PaymentDate { get; set; }
        //public virtual Product Product { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ZarinpallAuthority> ZarinpallAuthorities { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        internal class configuration : EntityTypeConfiguration<Order>
        {
            public configuration()
            {
                HasRequired(p => p.User).WithMany(t => t.Orders).HasForeignKey(p => p.UserId);
            }
        }

        [Display(Name="کد رهگیری پرداخت")]
        public string RefId { get; set; }

        [NotMapped]
        [Display(Name = "Amount", ResourceType = typeof(Resources.Models.Order))]
        public string AmountStr
        {
            get { return Amount.ToString("N0"); }
        }


        [NotMapped]
        [Display(Name = "DiscountAmount", ResourceType = typeof(Resources.Models.Order))]
        public string DiscountAmountStr
        {
            get
            {
                if (DiscountAmount == null)
                    return "0";
                return DiscountAmount.Value.ToString("N0");
            }
        }



        [NotMapped]
        [Display(Name = "TotalAmount", ResourceType = typeof(Resources.Models.Order))]
        public string TotalAmountStr
        {
            get { return TotalAmount.ToString("N0"); }
        }
    }
}
