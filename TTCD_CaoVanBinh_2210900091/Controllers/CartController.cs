using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TTCD_CaoVanBinh_2210900091.Models;

namespace TTCD_CaoVanBinh_2210900091.Controllers
{
    public class CartController : Controller
    {
        private TTCD_Cvb_2210900091Entities db = new TTCD_Cvb_2210900091Entities();

        // Khởi tạo hoặc lấy giỏ hàng từ sessiona
        private List<CartItem> GetCart()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // Action để thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id)
        {
            var product = db.PRODUCTs.Find(id); // Tìm sản phẩm theo ID trong database
            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(p => p.ProductId == id);

            // Nếu sản phẩm đã có trong giỏ, tăng số lượng
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.product_id,
                    ProductName = product.product_name,
                    ProductPrice = product.product_price,
                    Quantity = 1,
                    ProductImage = product.product_image
                });
            }

            return RedirectToAction("Index"); // Chuyển hướng đến trang giỏ hàng sau khi thêm
        }

        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(p => p.ProductId == id);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }

            return RedirectToAction("Index");
        }
        public ActionResult Checkout()
        {
            var cart = GetCart();

            if (cart.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return RedirectToAction("Index");
            }

            // Retrieve the member ID from session or authentication context
            var member = Session["Account"] as MEMBER; // Assuming "Account" stores MEMBER session data
            if (member == null)
            {
                ModelState.AddModelError("", "You must be logged in to place an order.");
                return RedirectToAction("Login", "Account");
            }

            // Create a new order and assign the member_id
            var order = new ORDER
            {
                member_id = member.member_id, // Set the member_id for the order
                order_date = DateTime.Now,
                total_price = cart.Sum(item => item.ProductPrice * item.Quantity)
            };

            db.ORDERS.Add(order);
            db.SaveChanges(); // Save to get the order ID for the details

            // Create order details for each cart item
            foreach (var item in cart)
            {
                var orderDetail = new ORDER_DETAIL
                {
                    order_id = order.order_id,
                    product_id = item.ProductId,
                    quantity = item.Quantity,
                    product_price = item.ProductPrice
                };
                db.ORDER_DETAIL.Add(orderDetail);
            }

            db.SaveChanges(); // Save order details

            // Clear the cart
            Session["Cart"] = null;

            return RedirectToAction("OrderConfirmation", new { id = order.order_id });
        }
        public ActionResult OrderConfirmation(int id)
        {
            var order = db.ORDERS.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
    }
}