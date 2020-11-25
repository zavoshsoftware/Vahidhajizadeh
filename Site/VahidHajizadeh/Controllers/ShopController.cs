﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using SmsIrRestful;
using ViewModels;

namespace Site.Controllers
{
    public class ShopController : Controller
    {

        //   private BaseViewModelHelper _baseHelper = new BaseViewModelHelper();
        private DatabaseContext db = new DatabaseContext();

        [Route("cart")]
        [HttpPost]
        public ActionResult AddToCart(string code, string qty)
        {
            SetCookie(code, qty);
            return Json("true", JsonRequestBehavior.AllowGet);
        }


        [Route("Basket")]
        public ActionResult Basket(string qty, string code)
        {
            BasketViewModel cart = new BasketViewModel();

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            if (productInCarts.Any())
                cart.IsPhysicalProduct = productInCarts.FirstOrDefault().Product.IsPhysicalProduct;
            else
                cart.IsPhysicalProduct = false;

            cart.Products = productInCarts;

            decimal subTotal = GetSubtotal(productInCarts);

            cart.SubTotal = subTotal.ToString("n0") + " تومان";

            decimal discountAmount = GetDiscount();

            cart.DiscountAmount = discountAmount.ToString("n0") + " تومان";

            cart.Total = (subTotal - discountAmount).ToString("n0");
            cart.MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup();
            cart.MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup();
            cart.footerBlogs = BaseViewModelHelper.GetFooterBlogs();
            return View(cart);
        }



        [AllowAnonymous]
        public ActionResult DiscountRequestPost(string coupon)
        {
            DiscountCode discount = db.DiscountCodes.Where(current => current.Code == coupon).FirstOrDefault();

            string result = CheckCouponValidation(discount);

            if (result != "true")
                return Json(result, JsonRequestBehavior.AllowGet);

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();
            decimal subTotal = GetSubtotal(productInCarts);

            decimal total = subTotal;

            DiscountHelper helper = new DiscountHelper();

            decimal discountAmount = helper.CalculateDiscountAmount(discount, total);

            SetDiscountCookie(discountAmount.ToString(), coupon);

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        ZarinPalHelper zp = new ZarinPalHelper();

        [AllowAnonymous]
        public ActionResult CheckUser(string company, string email, string cellNumber, string fullName)
        {
            try
            {
                cellNumber = cellNumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");

                bool isValidMobile = Regex.IsMatch(cellNumber, @"(^(09|9)[0-9][0-9]\d{7}$)|(^(09|9)[3][12456]\d{7}$)", RegexOptions.IgnoreCase);

                if (!isValidMobile)
                    return Json("invalidMobile", JsonRequestBehavior.AllowGet);

                if (!string.IsNullOrEmpty(email))
                {
                    bool isEmail = Regex.IsMatch(email,
                        @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                        RegexOptions.IgnoreCase);

                    if (!isEmail)
                        return Json("invalidEmail", JsonRequestBehavior.AllowGet);
                }
                User user = db.Users.FirstOrDefault(current => current.CellNum == cellNumber && current.IsActive);

                string code;

                if (user != null)
                {
                    code = user.Password;
                }

                else
                {
                    Guid userId = CreateUser(fullName, cellNumber, email);
                    int codeInt = CreateActivationCode(userId);
                    code = codeInt.ToString();
                }

                db.SaveChanges();

                SendSms(cellNumber, code);

                return Json("true", JsonRequestBehavior.AllowGet);
            }

            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }


        [AllowAnonymous]
        public ActionResult Finalize(string company, string email, string cellNumber, string activationCode, string address, string postal, string city)
        {
            try
            {
                cellNumber = cellNumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");
                activationCode = activationCode.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");

                User user = db.Users.OrderByDescending(c => c.CreationDate).FirstOrDefault(current => current.CellNum == cellNumber);

                if (user != null)
                {
                    ActivationCode activation = IsValidActivationCode(user.Id, activationCode);

                    if (activation != null)
                    {
                        ActivateUser(user, activationCode);
                        UpdateActivationCode(activation, null, null, null, null);
                        db.SaveChanges();



                        List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                        Order order = ConvertCoockieToOrder(productInCarts, user.Id, company, email, address, postal, city);

                        RemoveCookie();

                        string res = "";

                        if (order.TotalAmount == 0)
                            res = "freecallback?orderid=" + order.Id;

                        else
                            res = zp.ZarinPalRedirect(order, order.TotalAmount);

                        return Json(res, JsonRequestBehavior.AllowGet);
                    }

                    if (user.IsActive && user.Password == activationCode)
                    {
                        List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                        Order order = ConvertCoockieToOrder(productInCarts, user.Id, company, email, address, postal, city);

                        RemoveCookie();

                        string res = "";

                        if (order.TotalAmount == 0)
                            res = "freecallback?orderid=" + order.Id;

                        else
                            res = zp.ZarinPalRedirect(order, order.TotalAmount);

                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }

            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public ActivationCode IsValidActivationCode(Guid userId, string activationCode)
        {
            int code = Convert.ToInt32(activationCode);

            ActivationCode oActivationCode = db.ActivationCodes
                .FirstOrDefault(current => current.UserId == userId && current.Code == code && current.IsUsed == false
                                && current.IsActive == true && current.ExpireDate > DateTime.Now);

            if (oActivationCode != null)
                return oActivationCode;
            else
                return null;
        }

        public void ActivateUser(User user, string code)
        {
            user.IsActive = true;
            user.LastModifiedDate = DateTime.Now;
            user.Password = code;
            user.LastModifiedDate = DateTime.Now;
        }

        public void UpdateActivationCode(ActivationCode activationCode, string deviceId, string deviceModel,
            string OsType, string OsVersion)
        {
            activationCode.IsUsed = true;
            activationCode.UsingDate = DateTime.Now;
            activationCode.IsActive = false;
            activationCode.LastModifiedDate = DateTime.Now;

        }

        public void SendSms(string cellNumber, string code)
        {
            var token = new Token().GetToken("877bf267654c01afa079f268", "it66)%#teBC!@*&");

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = Convert.ToInt64(cellNumber),
                TemplateId = 24872,
                ParameterArray = new List<UltraFastParameters>()
                {
                    new UltraFastParameters()
                    {
                        Parameter = "VerificationCode" , ParameterValue = code
                    }
                }.ToArray()

            };

            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {

            }
            else
            {

            }
        }


        public Guid CreateUser(string fullName, string cellNumber, string email)
        {
            Guid roleId = new Guid("2d994cc5-6b11-453f-883e-94874978dce3");



            User user = new User()
            {
                CellNum = cellNumber,
                FullName = fullName,
                Email = email,
                RoleId = roleId,
                CreationDate = DateTime.Now,
                IsDeleted = false,
                Code = ReturnCode(),
                IsActive = false,
                Id = Guid.NewGuid()
            };

            db.Users.Add(user);
            db.SaveChanges();
            return user.Id;
        }

        public int ReturnCode()
        {
            Guid roleId = new Guid("2d994cc5-6b11-453f-883e-94874978dce3");

            User user = db.Users.Where(current => current.RoleId == roleId).OrderByDescending(current => current.Code).FirstOrDefault();
            if (user != null)
            {
                return user.Code + 1;
            }
            else
            {
                return 300001;
            }
        }

        public int CreateActivationCode(Guid userId)
        {
            DeactiveOtherActivationCode(userId);

            int code = RandomCode();
            ActivationCode activationCode = new ActivationCode();
            activationCode.UserId = userId;
            activationCode.Code = code;
            activationCode.ExpireDate = DateTime.Now.AddDays(2);
            activationCode.IsUsed = false;
            activationCode.IsActive = true;
            activationCode.CreationDate = DateTime.Now;
            activationCode.IsDeleted = false;

            db.ActivationCodes.Add(activationCode);
            return code;
        }

        public void DeactiveOtherActivationCode(Guid userId)
        {
            List<ActivationCode> activationCodes = db.ActivationCodes
                .Where(current => current.UserId == userId && current.IsActive == true).ToList();

            foreach (ActivationCode activationCode in activationCodes)
            {
                activationCode.IsActive = false;
                activationCode.LastModifiedDate = DateTime.Now;
            }

        }

        private Random random = new Random();
        public int RandomCode()
        {
            Random generator = new Random();
            String r = generator.Next(0, 100000).ToString("D5");
            return Convert.ToInt32(r);
        }


        public void RemoveCookie()
        {
            if (Request.Cookies["basket"] != null)
            {
                Response.Cookies["basket"].Expires = DateTime.Now.AddDays(-1);
            }
            if (Request.Cookies["discount"] != null)
            {
                Response.Cookies["discount"].Expires = DateTime.Now.AddDays(-1);
            }

        }
        public Order ConvertCoockieToOrder(List<ProductInCart> products, Guid userid, string company, string email, string address, string postal, string city)
        {
            try
            {
                Order order = new Order();

                foreach (ProductInCart product in products)
                {
                    int expiredNum = 0;


                    order.Id = Guid.NewGuid();
                    order.IsActive = true;
                    order.IsDeleted = false;
                    order.IsPaid = false;
                    order.CreationDate = DateTime.Now;
                    order.LastModifiedDate = DateTime.Now;
                    order.Code = FindeLastOrderCode() + 1;
                    order.UserId = userid;
                    order.CompanyName = company;
                    order.Email = email;
                    order.Address = address;
                    order.City = city;
                    order.PostalCode = postal;

                    decimal subtotal = GetSubtotal(products);

                    order.Amount = subtotal;

                    order.DiscountAmount = GetDiscount();

                    order.TotalAmount = Convert.ToDecimal(subtotal - order.DiscountAmount);

                    db.Orders.Add(order);
                    db.SaveChanges();


                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ProductId = product.Product.Id,
                        Quantity = product.Quantity,
                        RawAmount = product.Product.Amount * product.Quantity,
                        IsDeleted = false,
                        IsActive = true,
                        CreationDate = DateTime.Now,
                        OrderId = order.Id,
                        Amount = product.Product.Amount,

                    };


                    db.OrderDetails.Add(orderDetail);
                    db.SaveChanges();
                }
                return order;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int FindeLastOrderCode()
        {
            Order order = db.Orders.OrderByDescending(current => current.Code).FirstOrDefault();
            if (order != null)
                return order.Code;
            else
                return 999;
        }
        [AllowAnonymous]
        public string CheckCouponValidation(DiscountCode discount)
        {
            if (discount == null)
                return "Invald";

            //if (!discount.IsMultiUsing)
            //{

            //    int orderDiscount = UnitOfWork.OrderDiscountRepository.Get(current => current.DiscountCodeId == discount.Id && current.Order.UserId == user.Id).Count();
            //    if (orderDiscount > 1)
            //        return "Used";
            //}

            if (discount.ExpireDate < DateTime.Today)
                return "Expired";

            return "true";
        }


        public void SetDiscountCookie(string discountAmount, string discountCode)
        {
            HttpContext.Response.Cookies.Set(new HttpCookie("discount")
            {
                Name = "discount",
                Value = discountAmount + "/" + discountCode,
                Expires = DateTime.Now.AddDays(1)
            });
        }



        public decimal GetDiscount()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('/');
                    return Convert.ToDecimal(basketItems[0]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }


        public decimal GetSubtotal(List<ProductInCart> orderDetails)
        {
            decimal subTotal = 0;

            foreach (ProductInCart orderDetail in orderDetails)
            {
                decimal amount = orderDetail.Product.Amount;
                if (orderDetail.Product.IsInPromotion)
                    amount = orderDetail.Product.DiscountAmount.Value;

                subTotal = subTotal + (amount * orderDetail.Quantity);
            }

            return subTotal;
        }

        public List<ProductInCart> GetProductInBasketByCoockie()
        {
            List<ProductInCart> productInCarts = new List<ProductInCart>();

            string[] basketItems = GetCookie();

            if (basketItems != null)
            {
                for (int i = 0; i < basketItems.Length - 1; i++)
                {
                    string[] productItem = basketItems[i].Split('^');

                    int productCode = Convert.ToInt32(productItem[0]);

                    Product product = db.Products.FirstOrDefault(current => current.Code == productCode);

                    productInCarts.Add(new ProductInCart()
                    {
                        Product = product,
                        Quantity = Convert.ToInt32(productItem[1]),

                    });
                }
            }

            return productInCarts;
        }

        public void SetCookie(string code, string quantity)
        {
            string cookievalue = null;

            //if (Request.Cookies["basket"] != null)
            //{
            //    bool changeCurrentItem = false;

            //    cookievalue = Request.Cookies["basket"].Value;

            //    string[] coockieItems = cookievalue.Split('/');

            //    for (int i = 0; i < coockieItems.Length - 1; i++)
            //    {
            //        string[] coockieItem = coockieItems[i].Split('^');

            //        if (coockieItem[0] == code)
            //        {
            //            coockieItem[1] = (Convert.ToInt32(coockieItem[1]) + 1).ToString();
            //            changeCurrentItem = true;
            //            coockieItems[i] = coockieItem[0] + "^" + coockieItem[1];
            //            break;
            //        }
            //    }

            //    if (changeCurrentItem)
            //    {
            //        cookievalue = null;
            //        for (int i = 0; i < coockieItems.Length - 1; i++)
            //        {
            //            cookievalue = cookievalue + coockieItems[i] + "/";
            //        }

            //    }
            //    else
            //        cookievalue = cookievalue + code + "^" + quantity + "/";

            //}
            //else
            cookievalue = code + "^" + quantity + "/";

            HttpContext.Response.Cookies.Set(new HttpCookie("basket")
            {
                Name = "basket",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
        }

        public string[] GetCookie()
        {
            if (Request.Cookies["basket"] != null)
            {
                string cookievalue = Request.Cookies["basket"].Value;

                string[] basketItems = cookievalue.Split('/');

                return basketItems;
            }

            return null;
        }

        public long GetAmountByAuthority(string authority)
        {
            ZarinpallAuthority zarinpallAuthority =
                db.ZarinpallAuthorities.FirstOrDefault(current => current.Authority == authority);

            if (zarinpallAuthority != null)
                return Convert.ToInt64(zarinpallAuthority.Amount);

            return 0;
        }

        public Order GetOrderByAuthority(string authority)
        {
            ZarinpallAuthority zarinpallAuthority =
                db.ZarinpallAuthorities.FirstOrDefault(current => current.Authority == authority);

            if (zarinpallAuthority != null)
                return zarinpallAuthority.Order;

            else
                return null;
        }

        private String MerchantId = "8fa46f16-21af-11e7-a130-000c295eb8fc";

        [Route("callback")]
        public ActionResult CallBack(string authority, string status)
        {
            String Status = status;
            CallBackViewModel callBack = new CallBackViewModel()
            {
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),

            };

            if (Status != "OK")
            {
                callBack.IsSuccess = false;
            }

            else
            {
                try
                {
                    var zarinpal = ZarinPal.ZarinPal.Get();
                    zarinpal.DisableSandboxMode();
                    String Authority = authority;
                    long Amount = GetAmountByAuthority(Authority);


                    var verificationRequest = new ZarinPal.PaymentVerification(MerchantId, Amount, Authority);
                    var verificationResponse = zarinpal.InvokePaymentVerification(verificationRequest);
                    if (verificationResponse.Status == 100 || verificationResponse.Status == 101)
                    {
                        Order order = GetOrderByAuthority(authority);
                        if (order != null)
                        {
                            order.IsPaid = true;
                            order.PaymentDate = DateTime.Now;
                            order.RefId = verificationResponse.RefID;
                            order.LastModifiedDate = DateTime.Now;

                            db.SaveChanges();


                            callBack.IsSuccess = true;
                            callBack.OrderCode = order.Code.ToString();
                            callBack.RefrenceId = verificationResponse.RefID;

                            OrderDetail orderDetail = db.OrderDetails
                                .FirstOrDefault(current => current.OrderId == order.Id);

                            if (orderDetail != null)
                            {
                                Product product = db.Products.Find(orderDetail.ProductId);

                                if (product != null)
                                {
                                    product.BuyCount = product.BuyCount + 1;
                                    db.SaveChanges();

                                    string fileLink = null;

                                    if (!product.IsPhysicalProduct)
                                        fileLink = "https://vahidhajizadeh.com/" + product.FileUrl;

                                    callBack.DownloadLink = fileLink;

                                    ViewBag.Email = order.Email;

                                    if (!string.IsNullOrEmpty(order.Email))
                                        CreateEmail(order.Email, fileLink);

                                    if (orderDetail.Product.ProductGroup.UrlParam == "forms"||orderDetail.Product.ProductGroup.UrlParam== "course")
                                        callBack.ProductType = "form";
                                    else
                                        callBack.ProductType = "noForm";

                                    CreateEmailForAdmin(product.Title, Amount);

                                    if (product.IsPhysicalProduct)
                                        CreateEmailForAdminForPhysicalProduct(product.Title, order.User.CellNum,
                                            order.Address, order.City, order.PostalCode, order.User.FullName);

                                }
                            }

                        }
                        else
                        {
                            callBack.IsSuccess = false;
                            callBack.RefrenceId = "سفارش پیدا نشد";
                        }
                    }
                    else
                    {
                        callBack.IsSuccess = false;
                        callBack.RefrenceId = verificationResponse.Status.ToString();
                    }
                }
                catch (Exception e)
                {
                    callBack.IsSuccess = false;
                    callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
                }
            }
            return View(callBack);

        }



        public string SendEmailForCustomers()
        {
            List<Order> orders = db.Orders.Where(current => current.IsPaid == true).ToList();

            string ret = "";

            foreach (Order order in orders)
            {

                OrderDetail orderDetail = db.OrderDetails
                    .FirstOrDefault(current => current.OrderId == order.Id);

                if (orderDetail != null)
                {
                    Product product = db.Products.Find(orderDetail.ProductId);

                    if (product != null)
                    {

                        string fileLink = null;

                        if (!product.IsPhysicalProduct)
                            fileLink = "https://vahidhajizadeh.com/" + product.FileUrl;

                        CreateEmail(order.Email, fileLink);
                        ret = ret + order.Email + "/" + fileLink + ".................";
                    }
                }
            }

            return ret;
        }


        public void CreateEmailForAdmin(string productTitle, long amount)
        {
            Helpers.Message message = new Message();

            string email = "hajizadevahid797@gmail.com";
            //string email = "babaei.aho@gmail.com";
            string body = @"<html>
                 <head></head>
                <body dir='rtl'>
                <h1> خرید از وب سایت  وحید حاجی زاده</h1>
                <p> از وب سایت وحید حاجی زاده خریدی انجام شده است</p>
                <p>عنوان کالا: </p>" + productTitle + @"
                <p>مبلغ پرداخت شده: </p>" + amount + @"
                <h3>مجموعه قانون گستر</h3>
                <p>آدرس: تهران، خیابان شریعتی، سه راه طالقانی، جنب مبل پایتخت، پلاک 306، طبقه 2، واحد 4</p>
                <p>تلفن: 02177515152</p>
                <p>وب سایت: https://vahidhajizadeh.com/ </p>
                </body>
                </html> ";

            message.Send(email, "خرید از وب سایت  وحید حاجی زاده", body, "email");
        }

        public void CreateEmailForAdminForPhysicalProduct(string productTitle, string cellNumber, string address, string city, string postalCode, string fullName)
        {
            Helpers.Message message = new Message();

            string email = "akramfazli2011@gmail.com";
            // string email = "babaei.aho@gmail.com";
            string body = @"<html>
                 <head></head>
                <body dir='rtl'>
              <h1> خرید از وب سایت  وحید حاجی زاده</h1>
                <p> از وب سایت وحید حاجی زاده خریدی انجام شده است</p>
                <p>عنوان کالا: </p>" + productTitle + "<p>شماره موبایل کاربر: </p>" + cellNumber +
                          "<p>نام کاربر: </p>" + fullName + "<p>شهر: </p>" + city +
                          "<p>آدرس: </p>" + address + @"
                <h3>مجموعه قانون گستر</h3>
                <p>آدرس: تهران، خیابان شریعتی، سه راه طالقانی، جنب مبل پایتخت، پلاک 306، طبقه 2، واحد 4</p>
                <p>تلفن: 02177515152</p>
                <p>وب سایت: https://ghanongostar.com/ </p>
                </body>
                </html> ";

            message.Send(email, "خرید محصول فیزیکی از وب سایت وحید حاجی زاده", body, "email");
        }

        public void CreateEmail(string email, string file)
        {
            Helpers.Message message = new Helpers.Message();
            string body = GetMessageBody(file);

            message.Send(email, "خرید از وب سایت وحید حاجی زاده", body, "email");
        }

        public string GetMessageBody(string file)
        {
            string body;

            //body = @"<html>
            // <head></head>
            //<body dir='rtl'>
            //<h1> خرید از وب سایت وحید حاجی زاده</h1>
            //<p> با تشکر از خرید شما از وب سایت وحید حاجی زاده</p>
            //<p style='font-size:20px; color:red;'> لینک دانلود محصول خریداری شده:<a href= '" + file +
            //       @"' > دانلود</a></p>


            //<p>آدرس: تهران، خیابان شریعتی، سه راه طالقانی، جنب مبل پایتخت، پلاک 306، طبقه 2، واحد 4</p>
            //<p>تلفن: 02177515152</p>
            //<p>وب سایت: https://vahidhajizadeh.com/ </p>
            //</body>
            //</html> ";
            string downloadLinke = "";

            if (!string.IsNullOrEmpty(file))
                downloadLinke = @"<p style='font-size:20px; color:red;'> لینک دانلود محصول خریداری شده:<a href= '" +
                                file +
                                "' > دانلود</a></p>";
            body = @"<html>
                 <head></head>
                <body dir='rtl'>
                <h1> خرید از وب سایت وحید حاجی زاده</h1>
                <p> با تشکر از خرید شما از وب سایت وحید حاجی زاده</p>" + downloadLinke +


                @"<p>آدرس: تهران، خیابان شریعتی، سه راه طالقانی، جنب مبل پایتخت، پلاک 306، طبقه 2، واحد 4</p>
                <p>تلفن: 02177515152</p>
                <p>وب سایت: https://vahidhajizadeh.com/ </p>
                </body>
                </html> ";



            return body;
        }

        [Route("freecallback")]
        public ActionResult CallBackFree(Guid orderId)
        {
            CallBackViewModel callBack = new CallBackViewModel()
            {
                MenuProductGroups = BaseViewModelHelper.GetMenuProductGroup(),
                MenuBlogGroups = BaseViewModelHelper.GetMenuBlogGroup(),
                footerBlogs = BaseViewModelHelper.GetFooterBlogs(),
            };

            try
            {
                Order order = db.Orders.Find(orderId);
                if (order != null)
                {
                    order.IsPaid = true;
                    order.PaymentDate = DateTime.Now;
                    order.LastModifiedDate = DateTime.Now;

                    db.SaveChanges();

                    callBack.IsSuccess = true;
                    callBack.OrderCode = order.Code.ToString();


                    OrderDetail orderDetail = db.OrderDetails
                        .FirstOrDefault(current => current.OrderId == order.Id);

                    if (orderDetail != null)
                    {
                        Product product = db.Products.Find(orderDetail.ProductId);

                        if (product != null)
                        {
                            string fileLink = null;

                            if (!product.IsPhysicalProduct)
                                fileLink = "https://vahidhajizadeh.com/" + product.FileUrl;

                            callBack.DownloadLink = fileLink;

                            ViewBag.Email = order.Email;

                            if (!string.IsNullOrEmpty(order.Email))
                                CreateEmail(order.Email, fileLink);


                            if (orderDetail.Product.ProductGroup.UrlParam == "forms")
                                callBack.ProductType = "form";
                            else
                                callBack.ProductType = "noForm";

                            CreateEmailForAdmin(product.Title, 0);
                        }
                    }
                }
                else
                {
                    callBack.IsSuccess = false;
                    callBack.RefrenceId = "سفارش پیدا نشد";
                }

            }
            catch (Exception e)
            {
                callBack.IsSuccess = false;
                callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
            }

            return View(callBack);

        }



    }
}