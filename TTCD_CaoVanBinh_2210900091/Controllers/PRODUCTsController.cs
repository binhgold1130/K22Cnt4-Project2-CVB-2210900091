using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TTCD_CaoVanBinh_2210900091.Models;

namespace TTCD_CaoVanBinh_2210900091.Controllers
{
    public class PRODUCTsController : Controller
    {
        private TTCD_Cvb_2210900091Entities db = new TTCD_Cvb_2210900091Entities();

        // GET: PRODUCTs
        public ActionResult ProIndex()
        {
            return View(db.PRODUCTs.ToList());
        }


        /// GET: PRODUCTs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCT pRODUCT = db.PRODUCTs.Find(id);
            if (pRODUCT == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCT);
        }
        [HttpGet]
        public ActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("ProIndex"); // Trở lại trang danh sách nếu không có từ khóa
            }

            var products = db.PRODUCTs
                              .Where(p => p.product_name.Contains(query)) // Tìm kiếm theo tên sản phẩm
                              .ToList();

            return View(products); // Trả về view Search với kết quả
        }


        // GET: Shopping Cart
        public ActionResult Cart()
        {
            List<ORDER_DETAIL> cart = Session["Cart"] as List<ORDER_DETAIL> ?? new List<ORDER_DETAIL>();
            return View(cart);
        }

        // Method to remove a product from the cart
        public ActionResult RemoveFromCart(int id)
        {
            List<ORDER_DETAIL> cart = Session["Cart"] as List<ORDER_DETAIL>;
            if (cart != null)
            {
                var productToRemove = cart.FirstOrDefault(p => p.product_id == id);
                if (productToRemove != null)
                {
                    cart.Remove(productToRemove);
                }
                Session["Cart"] = cart;
            }

            return RedirectToAction("Cart");
        }

        // Method to add product to the cart
        public ActionResult AddToCart(int id)
        {
            var product = db.PRODUCTs.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            List<ORDER_DETAIL> cart = Session["Cart"] as List<ORDER_DETAIL> ?? new List<ORDER_DETAIL>();
            var existingProduct = cart.FirstOrDefault(p => p.product_id == id);
            if (existingProduct != null)
            {
                existingProduct.quantity++;
            }
            else
            {
                cart.Add(new ORDER_DETAIL
                {
                    product_id = product.product_id,
                    PRODUCT = product,
                    quantity = 1,
                    product_price = product.product_price
                });
            }

            Session["Cart"] = cart;
            return RedirectToAction("Cart");
        }

        // Method to proceed to checkout and create an order
        public ActionResult Checkout()
        {
            List<ORDER_DETAIL> cart = Session["Cart"] as List<ORDER_DETAIL>;
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Cart"); // Redirect to cart if it's empty
            }

            // Create a new order
            var order = new ORDER
            {
                order_date = DateTime.Now,
                order_status = 1, // Set to 1 for pending
                product_price = cart.Sum(i => i.product_price * i.quantity),
                total_price = cart.Sum(i => i.product_price * i.quantity), // Total price of the order
                member_id = 1,/* Set the member ID here, e.g., from the logged-in user's session */
            };

            // Add the order to the database
            db.ORDERS.Add(order);
            db.SaveChanges();

            // Create order details for each item in the cart
            foreach (var item in cart)
            {
                var orderDetail = new ORDER_DETAIL
                {
                    order_id = order.order_id, // Link to the new order
                    product_id = item.product_id,
                    quantity = item.quantity,
                    product_price = item.product_price
                };

                db.ORDER_DETAIL.Add(orderDetail);
            }

            db.SaveChanges();

            // Clear the cart after successful checkout
            Session["Cart"] = null;

            // Redirect to order confirmation page or cart
            return RedirectToAction("OrderConfirmation", new { id = order.order_id });
        }

        // Add an action to confirm the order
        public ActionResult OrderConfirmation(int id)
        {
            var order = db.ORDERS.Include("ORDER_DETAIL").FirstOrDefault(o => o.order_id == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

    }
}
