using Project_3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using PagedList;
using Project_3.Models;

namespace Project_3.Areas.Admin.Controllers
{
    public class RoleController : BaseController
    {
        // GET: Admin/Role
        public ActionResult Index()
        {
            var data = Common.Db.ROLES.ToList();
            return View(data);
        }

        #region ManagerRole
        // GET: Admin/Role/ManagerRole/id
        public ActionResult ManagerRole(int? id, string searchPage, string search, int? page)
        {
            var checkRole = Common.Db.ROLES.ToList().Where(r => r.RoleId == id).FirstOrDefault();
            if (checkRole == null || id == 1)
            {
                return HttpNotFound();
            }    
            var data = new List<REL_MANAGER_ROLES>();
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
                data = Common.Db.REL_MANAGER_ROLES.Include(x => x.MANAGER).Include(x => x.ROLES).ToList()
                    .Where(x => x.RoleId == id && (x.MANAGER.FullName.ToLower().Contains(text) || x.MANAGER.Username.Contains(text))).ToList();
            }
            else
            {
                data = Common.Db.REL_MANAGER_ROLES.Include(x => x.MANAGER).Include(x => x.ROLES).ToList().Where(x => x.RoleId == id).ToList();
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["RoleId"] = id;
            ViewData["RoleName"] = checkRole.RoleName;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        // POST: Admin/Role/ManagerRole/id
        [HttpPost]
        public ActionResult ManagerRole(REL_MANAGER_ROLES input, int? id)
        {
            var checkRole = Common.Db.ROLES.ToList().Where(r => r.RoleId == id).FirstOrDefault();
            if (checkRole == null || id == 1)
            {
                return HttpNotFound();
            }

            var data = Common.Db.REL_MANAGER_ROLES.ToList().Where(x => x.RoleId == id && x.ManagerId == input.ManagerId).FirstOrDefault();
            if (data != null)
            {
                Common.Db.REL_MANAGER_ROLES.Remove(data);
                Common.Db.SaveChanges();
            }
            return RedirectToAction("ManagerRole", "Role", new { page = Common.Page, searchPage = Common.Search });
        }
        #endregion

        #region AddManagerToRole
        // GET: Admin/Role/AddManagerToRole/id
        public ActionResult AddManagerToRole(int? id, string searchPage, string search, int? page)
        {
            var checkRole = Common.Db.ROLES.ToList().Where(r => r.RoleId == id).FirstOrDefault();
            if (checkRole == null || id == 1)
            {
                return HttpNotFound();
            }
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
            ViewData["RoleId"] = id;
            ViewData["RoleName"] = checkRole.RoleName;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        // POST: Admin/Role/AddManagerToRole/id
        [HttpPost]
        public ActionResult AddManagerToRole(REL_MANAGER_ROLES input, int? id)
        {
            var checkRole = Common.Db.ROLES.ToList().Where(r => r.RoleId == id).FirstOrDefault();
            if (checkRole == null || id == 1)
            {
                return HttpNotFound();
            }

            var checkManager = Common.Db.REL_MANAGER_ROLES.ToList().Where(x => x.RoleId == input.RoleId && x.ManagerId == input.ManagerId).FirstOrDefault();
            if (checkManager != null)
            {
                Common.Fail = checkManager.MANAGER.FullName + " already has role " + checkRole.RoleName;
                return RedirectToAction("AddManagerToRole", "Role");
            }

            Common.Db.REL_MANAGER_ROLES.Add(input);
            Common.Db.SaveChanges();
            Common.Success = "Successfully added role " + checkRole.RoleName + " to " + input.MANAGER.FullName + ".";
            return RedirectToAction("AddManagerToRole", "Role", new { page = Common.Page, searchPage = Common.Search });
        }
        #endregion
    }
}