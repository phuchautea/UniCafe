using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UniCafe.Models;
using UniCafe.Data;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Net.PeerToPeer;
using System.Net;
using System.Data.Entity;

namespace UniCafe.Controllers
{
    public class RoleController : MasterController<Role>
    {
        // GET: Role
        public ActionResult Index()
        {
            var listRole = GetAll().ToList();
            return View(listRole);
        }

        // POST: Role/Create
        [HttpPost]
        public ActionResult Create(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var roleName = formCollection["Name"];
                var checkRole = Context.Roles.Count(x => x.Name == roleName);
                if (string.IsNullOrEmpty(formCollection["Name"]))
                {
                    errors.Add("Vui lòng nhập tên role.");
                }
                if (checkRole > 0)
                {
                    errors.Add("Tên role đã tồn tại.");
                }
                if (errors.Count == 0)
                {
                    
                    Role role = new Role();
                    role.Name = roleName;
                    Context.Roles.Add(role);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index");
        }
        public ActionResult SetRole()
        {
            var roles = Context.Roles.ToList();
            var users = Context.Users.ToList();
            // Create a new list to store the role-user mappings
            var roleUserList = new List<AspNetUserRoles>();

            foreach (var role in roles)
            {
                // Get all the users assigned to the role
                var userIds = Context.Set<IdentityUserRole>()
                    .Where(ur => ur.RoleId == role.Id)
                    .Select(ur => ur.UserId)
                    .ToList();

                // Get all the users assigned to the role
                var roleUsers = Context.Set<ApplicationUser>()
                    .Where(u => userIds.Contains(u.Id))
                    .ToList();

                // Add the role-user mappings to the list
                roleUserList.AddRange(roleUsers.Select(u => new AspNetUserRoles { RoleId = role.Name, UserId = u.UserName }));
            }
            ViewBag.Roles = roles;
            ViewBag.Users = users;
            return View(roleUserList);
        }
        [HttpPost]
        public ActionResult SetRole(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var RoleId = formCollection["RoleId"];
                var UserId = formCollection["UserId"];
                var getRole = Context.Roles.FirstOrDefault(x => x.Id == RoleId);
                var getUser = Context.Users.FirstOrDefault(x => x.Id == UserId);
                if (getRole == null)
                {
                    errors.Add("Không tìm thấy Role này.");
                }
                if (getUser == null)
                {
                    errors.Add("Không tìm thấy User này.");
                }
                if (errors.Count == 0)
                {
                    IdentityUserRole userRole = new IdentityUserRole();
                    userRole.UserId = getUser.Id;
                    userRole.RoleId = getRole.Id;
                    getUser.Roles.Add(userRole);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("SetRole");
        }
        // GET: Role/DeleteSetRole/5
        public ActionResult DeleteSetRole(string UserName, string Role)
        {
            var getRole = Context.Roles.FirstOrDefault(x => x.Id == Role);
            var getUser = Context.Users.FirstOrDefault(x => x.UserName == UserName);
            var userRole = Context.Set<IdentityUserRole>().FirstOrDefault(ur => ur.UserId == getUser.Id && ur.RoleId == getRole.Id);

            return View(userRole);
        }

        // POST: Role/DeleteSetRole/5
        [HttpPost]
        public ActionResult DeleteSetRole(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var RoleId = formCollection["RoleId"];
                var UserName = formCollection["UserName"];

                var getRole = Context.Roles.FirstOrDefault(x => x.Id == RoleId);
                var getUser = Context.Users.FirstOrDefault(x => x.UserName == UserName);
                if (getRole == null)
                {
                    errors.Add("Không tìm thấy Role này.");
                }
                if (getUser == null)
                {
                    errors.Add("Không tìm thấy User này.");
                }
                if (errors.Count == 0)
                {
                    // Retrieve the record that you want to delete
                    var userRole = Context.Set<IdentityUserRole>().FirstOrDefault(ur => ur.UserId == getUser.Id && ur.RoleId == getRole.Id);

                    if (userRole != null)
                    {
                        // Remove the record from the AspNetUserRoles table
                        Context.Set<IdentityUserRole>().Remove(userRole);

                        // Save the changes to the database
                        Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("SetRole");
        }
    }
}
