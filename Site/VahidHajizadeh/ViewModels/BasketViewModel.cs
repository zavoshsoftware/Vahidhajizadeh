﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class BasketViewModel : _BaseViewModel
    {
        public List<ProductInCart> Products { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string DiscountAmount { get; set; }

        public bool IsPhysicalProduct { get; set; }
    }

    public class ProductInCart
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string RowAmount
        {
            get
            {
                if (Product.IsInPromotion && Product.DiscountAmount != null)
                    return (Product.DiscountAmount.Value * Quantity).ToString("n0") + " تومان";

                return (Product.Amount * Quantity).ToString("n0") + " تومان";
            }
        }
    }
}