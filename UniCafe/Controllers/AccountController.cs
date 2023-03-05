using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer;
using System.Web;
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