using PagedList;
using Project_3.Data;
using Project_3.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Project_3.Areas.Admin.Controllers
{
    public class ManagerController : BaseController
    {
        #region Index
        // GET: Admin/Manager
        public ActionResult Index(string searchPage, string search, int? page)
        {
            var data = new List<MANAGER>();
            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = searchPage;
            }
            if (!string.IsNullOrEmpty(search))
            {
                string text = search.ToLower();
                data = Common.Db.MANAGER.ToList().Where(m => m.FullName.ToLower().Contains(text) || m.Username.Contains(text)).ToList();
            }
            else
            {
                data = Common.Db.MANAGER.ToList();
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region Create
        // GET: Admin/Manager/Create
        public ActionResult Create()
        {
            return View();
        }

        public bool CheckValidateCreate(MANAGER manager)
        {
            if (string.IsNullOrWhiteSpace(manager.Username))
            {
                ModelState.AddModelError("Username", "Please enter Username");
            }
            if (string.IsNullOrWhiteSpace(manager.Password))
            {
                ModelState.AddModelError("Password", "Please enter Password");
            }
            if (manager.Birth > DateTime.Now)
            {
                ModelState.AddModelError("Birth", "Invalid date of birth.");
            }
            if (!string.IsNullOrWhiteSpace(manager.Username) && !Regex.IsMatch(manager.Username, "^[a-zA-Z][\\w.]{2,14}$"))
            {
                ModelState.AddModelError("Username", "Username must start with a letter and be between 3-15 characters. Usernames can contain special characters: underscores and periods.");
            }
            if (!string.IsNullOrWhiteSpace(manager.Password) && !Regex.IsMatch(manager.Password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$"))
            {
                ModelState.AddModelError("Password", "Password minimum eight characters, at least one uppercase letter, one lowercase letter, and one number.");
            }

            var username = Common.Db.MANAGER.ToList().Any(m => m.Username == manager.Username);
            if (username)
            {
                ModelState.AddModelError("Username", "Username already exists");
            }

            var email = Common.Db.MANAGER.ToList().Any(m => m.Email == manager.Email);
            if (email)
            {
                ModelState.AddModelError("Email", "Email already exists");
            }

            var phoneNumber = Common.Db.MANAGER.ToList().Any(m => m.PhoneNumber == manager.PhoneNumber);
            if (phoneNumber)
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number already exists");
            }
            return true;
        }

        // POST: Admin/Manager/Create
        [HttpPost]
        public ActionResult Create(MANAGER manager)
        {
            if (CheckValidateCreate(manager) && ModelState.IsValid)
            {
                // Add Manager
                manager.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(manager.FullName.ToLower());
                manager.Username = manager.Username.ToLower();
                manager.Password = Common.Hash(manager.Password);
                Common.Db.MANAGER.Add(manager);
                Common.Db.SaveChanges();
                ViewData["Message"] = "Create successful employee";
            }
            return View(manager);
        }
        #endregion

        #region Update
        // GET: Admin/Manager/Update
        public ActionResult Update(int? id)
        {
            var data = Common.Db.MANAGER.ToList().Where(c => c.ManagerId == id).FirstOrDefault();
            if (data == null || (Common.CheckRoleAdmin(id) && !Common.CheckRole(1)))
            {
                return HttpNotFound();
            }
            return View(data);
        }

        public bool CheckValidateUpdate(MANAGER manager, int? id)
        {
            if (manager.Birth > DateTime.Now)
            {
                ModelState.AddModelError("Birth", "Invalid date of birth.");
            }

            var email = Common.Db.MANAGER.ToList().Any(m => m.Email == manager.Email && m.ManagerId != id);
            if (email)
            {
                ModelState.AddModelError("Email", "Email already exists");
            }

            var phoneNumber = Common.Db.MANAGER.ToList().Any(m => m.PhoneNumber == manager.PhoneNumber && m.ManagerId != id);
            if (phoneNumber)
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number already exists");
            }
            return true;
        }

        // POST: Admin/Manager/Update
        [HttpPost]
        public ActionResult Update(MANAGER manager, int? id)
        {
            var data = Common.Db.MANAGER.ToList().Where(c => c.ManagerId == id).FirstOrDefault();
            if (data == null || (Common.CheckRoleAdmin(id) && !Common.CheckRole(1)))
            {
                return HttpNotFound();
            }
            if (CheckValidateUpdate(manager, id) && ModelState.IsValid)
            {
                data.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(manager.FullName.ToLower());
                data.Birth = manager.Birth;
                data.Gender = manager.Gender;
                data.PhoneNumber = manager.PhoneNumber;
                data.Email = manager.Email;
                data.Address = manager.Address;

                Common.Db.SaveChanges();
                ViewData["Message"] = "Update successful employee";
            }
            return View(data);
        }
        #endregion

        #region ResetPassword
        // GET: Admin/Manager/ResetPassword
        public ActionResult ResetPassword(int? id)
        {
            var data = Common.Db.MANAGER.ToList().Where(c => c.ManagerId == id).FirstOrDefault();
            if (data == null || (Common.CheckRoleAdmin(id) && !Common.CheckRole(1)))
            {
                return HttpNotFound();
            }
            return View(data);
        }

        // POST: Admin/Employee/ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string pass, int? id)
        {
            var data = Common.Db.MANAGER.ToList().Where(c => c.ManagerId == id).FirstOrDefault();
            if (data == null || (Common.CheckRoleAdmin(id) && !Common.CheckRole(1)))
            {
                return HttpNotFound();
            }
            if (!Regex.IsMatch(pass, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$"))
            {
                ModelState.AddModelError("pass", "Password minimum eight characters, at least one uppercase letter, one lowercase letter, and one number.");
                return View(data);
            }
            data.Password = Common.Hash(pass);
            Common.Db.SaveChanges();
            ViewData["Message"] = "Reset Password successful";
            return View(data);
        }
        #endregion
    }
}