using PagedList;
using Project_3.Data;
using Project_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;

namespace Project_3.Areas.Admin.Controllers
{
    public class HumanResourcesController : BaseController
    {
        // GET: Admin/HR
        public ActionResult Index(string searchPage, string search, int? page)
        {
            var data = new List<RESULTS>();
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
                data = Common.Db.RESULTS.Include(r => r.STATUS_RESULTS).Include(r => r.CANDIDATES).ToList()
                    .Where(r => r.StatusId == 2 && (r.CANDIDATES.FullName.ToLower().Contains(text) || r.CANDIDATES.Username.Contains(text))).ToList();
            }
            else
            {
                data = Common.Db.RESULTS.Include(r => r.STATUS_RESULTS).Include(r => r.CANDIDATES).ToList().Where(r => r.StatusId == 2).ToList();
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
    }
}