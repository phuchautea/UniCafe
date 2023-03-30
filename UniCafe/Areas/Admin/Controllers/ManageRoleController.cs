using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer;
using System.Web;
using System.Web.Mvc;
using UniCafe.Controllers;
using UniCafe.Data;
using UniCafe.Models;

namespace UniCafe.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ManageRoleController : BaseController<Role>
    {
        // GET: Admin/ManageRole
        public ActionResult Index()
        {
            var roles = GetAll().ToList();
            return View(roles);
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection)
        {
            List<string> errors = new List<string>();
            try
            {
                var name = formCollection["name"];
                var checkRole = Context.Roles.Count(x => x.Name == name);
                if (string.IsNullOrEmpty(name))
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
                    role.Name = name;
                    Context.Roles.Add(role);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            TempData["Errors"] = errors;
            return RedirectToAction("Index", "ManageRole");
        }
        [HttpPost]
        public ActionResult Delete(Guid? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "ID không hợp lệ" });
            }
            var role = Context.Roles.FirstOrDefault(x => x.Id.Equals(Id.ToString()));
            if (role == null)
            {
                return Json(new { success = false, message = "Role không tồn tại" });
            }
            Remove((Role)role);
            return Json(new { success = true, message = "Xóa role thành công" });
        }
        public ActionResult SetRole()
        {
            var roles = Context.Roles.ToList();
            var users = Context.Users.ToList();
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
                roleUserList.AddRange(roleUsers.Select(u => new AspNetUserRoles { RoleId = role.Id, UserId = u.Id }));
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
            return RedirectToAction("SetRole", "ManageRole");
        }
        [HttpPost]
        public ActionResult DeleteSetRole(Guid? UserId, Guid? RoleId)
        {
            try
            {
                if (RoleId == null || UserId == null)
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
                }
                var getRole = Context.Roles.FirstOrDefault(x => x.Id.Equals(RoleId.ToString()));
                var getUser = Context.Users.FirstOrDefault(x => x.Id.Equals(UserId.ToString()));

                if (getRole == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy Role này." });
                }
                if (getUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy User này." });
                }
                var userRole = Context.Set<IdentityUserRole>().FirstOrDefault(ur => ur.UserId == getUser.Id && ur.RoleId == getRole.Id);

                if (userRole != null)
                {
                    // Remove the record from the AspNetUserRoles table
                    Context.Set<IdentityUserRole>().Remove(userRole);

                    // Save the changes to the database
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            return Json(new { success = true, message = "Xóa set role thành công" });
        }
    }
}