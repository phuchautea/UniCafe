using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using UniCafe.Models;

namespace UniCafe.Data
{
    public class AccountController : Controller
    {
        private ApplicationDbContext _context;
        public AccountController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public static bool ValidateVNPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("+84", "0");
            Regex regex = new Regex(@"^(0)(86|96|97|98|32|33|34|35|36|37|38|39|91|94|83|84|85|81|82|90|93|70|79|77|76|78|92|56|58|99|59|55|87)\d{7}$");
            return regex.IsMatch(phoneNumber);
        }
        public bool ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }
        public bool ValidatePassword(string password)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(password);
        }
        [HttpPost]
        public ActionResult Register(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var Email = formCollection["Email"];
                var Password = formCollection["PasswordHash"];
                var PhoneNumber = formCollection["PhoneNumber"];
                var UserName = formCollection["UserName"];

                var checkEmail = _context.Users.Count(x => x.Email == Email);
                var checkPhoneNumber = _context.Users.Count(x => x.PhoneNumber == PhoneNumber);
                var checkUserName = _context.Users.Count(x => x.UserName == UserName);
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(UserName))
                {
                    errors.Add("Vui lòng nhập đầy đủ thông tin.");
                }
                if (checkEmail > 0)
                {
                    errors.Add("Email đã tồn tại.");
                }
                if (checkPhoneNumber > 0)
                {
                    errors.Add("Số điện thoại đã tồn tại.");
                }
                if (ValidateVNPhoneNumber(PhoneNumber) != true)
                {
                    errors.Add("Số điện thoại không hợp lệ.");
                }
                if (ValidatePassword(Password) == false)
                {
                    errors.Add("Mật khẩu ít nhất 8 kí tự (Có 1 chữ hoa + thường + số + kí tự đặc biệt");
                }
                if (ValidateEmail(Email) == false)
                {
                    errors.Add("Email không hợp lệ");
                }
                if (checkUserName > 0)
                {
                    errors.Add("Username đã tồn tại.");
                }
                if (errors.Count == 0)
                {
                    // Create a UserStore
                    var userStore = new UserStore<ApplicationUser>(_context);

                    // Create a UserManager with the UserStore
                    var userManager = new UserManager<ApplicationUser>(userStore);
                    var user = new ApplicationUser { UserName = UserName, Email = Email, PhoneNumber = PhoneNumber };

                    var result = userManager.Create(user, Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        errors.Add(result.Errors.FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return View();
        }
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var UserName = formCollection["UserName"];
                var Password = formCollection["Password"];
                var userStore = new UserStore<IdentityUser>();
                var userManager = new UserManager<IdentityUser>(userStore);
                var user = userManager.Find(UserName, Password);
                if (user != null)
                {
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;

                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                    authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, userIdentity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    errors.Add("Sai tài khoản hoặc mật khẩu");
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return View();
        }
        public ActionResult Logout()
        {
            var AuthenticationManager = HttpContext.GetOwinContext().Authentication;
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}