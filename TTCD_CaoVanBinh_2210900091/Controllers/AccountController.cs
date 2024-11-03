using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using TTCD_CaoVanBinh_2210900091.Models;

namespace TTCD_CaoVanBinh_2210900091.Controllers

{
    public class AccountController : Controller
    {
        private TTCD_Cvb_2210900091Entities db = new TTCD_Cvb_2210900091Entities(); // Đảm bảo DbContext đúng

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa mật khẩu để kiểm tra với cơ sở dữ liệu
                string hashedPassword = HashPassword(model.member_pass);

                // Kiểm tra thông tin đăng nhập từ cơ sở dữ liệu với mật khẩu đã mã hóa
                var member = db.MEMBERs
                    .FirstOrDefault(m => m.member_username == model.member_username && m.member_pass == model.member_pass);

                if (member != null)
                {
                    Session["Account"] = member; // Lưu toàn bộ đối tượng MEMBER
                    FormsAuthentication.SetAuthCookie(member.member_username, false);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        public ActionResult AdminLogin(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = HashPassword(model.admin_pass);
                var admin = db.QUAN_TRI
                    .FirstOrDefault(a => a.admin_username == model.admin_username && a.admin_pass == model.admin_pass);

                if (admin != null)
                {
                    Session["AdminAccount"] = admin;
                    FormsAuthentication.SetAuthCookie(admin.admin_username, false);
                    return RedirectToAction("Index", "Admin");
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View("Login", model);
        }

        // Logout method
        public ActionResult Logout()
        {
            Session["Account"] = null;
            Session["AdminAccount"] = null; // Clear the admin session
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // Phương thức mã hóa mật khẩu
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public ActionResult Index()
        {
            return View(db.MEMBERs.ToList());
        }

        // GET: MEMBERs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MEMBER mEMBER = db.MEMBERs.Find(id);
            if (mEMBER == null)
            {
                return HttpNotFound();
            }
            return View(mEMBER);
        }
        // GET: MEMBERs/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "member_id,member_name,member_username,member_pass,dia_chi,member_phone,member_email,ngay_sinh,ngay_cap_nhap,gioi_tinh,tich_diem")] MEMBER mEMBER)
        {
            if (ModelState.IsValid)
            {
                db.MEMBERs.Add(mEMBER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mEMBER);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(MEMBER model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                var existingUser = db.MEMBERs.FirstOrDefault(m => m.member_username == model.member_username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Đặt giá trị mặc định cho những trường không nhập vào

                model.tich_diem = 0;


                // Thêm thành viên mới vào cơ sở dữ liệu
                db.MEMBERs.Add(model);
                db.SaveChanges();


                // Sau khi đăng ký thành công, chuyển hướng về trang đăng nhập
                return RedirectToAction("LoginSuccess", "Account");
            }

            return RedirectToAction("RegisterSuccess");
            // Nếu có lỗi, quay lại trang đăng ký
            return View(model);
        }


        public ActionResult AccountDetails()
        {
            // Lấy dữ liệu tài khoản từ Session
            var account = Session["Account"] as MEMBER; // Sử dụng model MEMBER mà bạn đã lưu vào session

            if (account == null)
            {
                return RedirectToAction("Login"); // Nếu chưa đăng nhập, chuyển đến trang đăng nhập
            }

            return View(account); // Trả về trang AccountDetails với thông tin tài khoản
        }

        public ActionResult EditAccount()
        {
            // Lấy dữ liệu tài khoản từ Session
            var account = Session["Account"] as MEMBER;

            if (account == null)
            {
                return RedirectToAction("Login"); // Nếu chưa đăng nhập, chuyển đến trang đăng nhập
            }

            return View(account); // Trả về view với thông tin hiện tại của tài khoản
        }
        public ActionResult Create(MEMBER model)
        {
            if (ModelState.IsValid)
            {
                model.ngay_cap_nhap = DateTime.Now; // Automatically set to the current date and time
                db.MEMBERs.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // POST: Chỉnh sửa thông tin tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(MEMBER model)
        {
            if (ModelState.IsValid)
            {
                var member = db.MEMBERs.Find(model.member_id);

                if (member != null)
                {
                    // Update other fields
                    member.member_name = model.member_name;
                    member.member_username = model.member_username;
                    member.member_pass = model.member_pass;
                    member.dia_chi = model.dia_chi;
                    member.member_phone = model.member_phone;
                    member.member_email = model.member_email;
                    member.ngay_sinh = model.ngay_sinh;
                    member.gioi_tinh = model.gioi_tinh;
                    member.tich_diem = model.tich_diem;

                    // Automatically set the current date and time for ngay_cap_nhat
                    member.ngay_cap_nhap = DateTime.Now;

                    db.Entry(member).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    // Update the session if needed
                    Session["Account"] = member;

                    return RedirectToAction("AccountDetails"); // Redirect to account details after saving
                }
            }

            return View(model); // Return the view with the model if validation fails
        }
    }
}