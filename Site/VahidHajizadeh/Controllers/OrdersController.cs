using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;
using ViewModels;

namespace VahidHajizadeh.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OrdersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Where(o => o.IsDeleted == false)
                .OrderByDescending(o => o.CreationDate).Include(o => o.User);

            List<OrderListViewModel> orderList = new List<OrderListViewModel>();

            foreach (Order order in orders)
            {
                string productTitle = "";
                OrderDetail orderDetail = db.OrderDetails.FirstOrDefault(c => c.OrderId == order.Id);
                if (orderDetail != null)
                {
                    Product product = db.Products.FirstOrDefault(c => c.Id == orderDetail.ProductId);

                    if (product != null)
                        productTitle = product.Title;
                }

                orderList.Add(new OrderListViewModel()
                {
                    Id = order.Id,
                    Code = order.Code,
                    IsPaid = order.IsPaid,
                    Amount = order.AmountStr,
                    TotalAmount = order.TotalAmountStr,
                    FullName = order.User.FullName,
                    CreationDate = order.CreationDateStr,
                    PaymentDate = order.PaymentDate,
                    ResCode = order.RefId,
                    ProductTitle = productTitle
                });
            }

            return View(orderList);
        }

        // GET: Orders/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,UserId,Amount,TotalAmount,DiscountAmount,CityId,Address,PostalCode,Email,CompanyName,IsPaid,PaymentDate,RefId,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] Order order)
        {
            if (ModelState.IsValid)
            {
				order.IsDeleted=false;
				order.CreationDate= DateTime.Now; 
                order.Id = Guid.NewGuid();
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

           // ViewBag.CityId = new SelectList(db.Cities, "Id", "Title", order.CityId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CityId = new SelectList(db.Cities, "Id", "Title", order.CityId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,UserId,Amount,TotalAmount,DiscountAmount,CityId,Address,PostalCode,Email,CompanyName,IsPaid,PaymentDate,RefId,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] Order order)
        {
            if (ModelState.IsValid)
            {
				order.IsDeleted=false;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           // ViewBag.CityId = new SelectList(db.Cities, "Id", "Title", order.CityId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Order order = db.Orders.Find(id);
			order.IsDeleted=true;
			order.DeletionDate=DateTime.Now;
 
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
