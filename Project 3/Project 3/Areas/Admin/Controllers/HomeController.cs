using Project_3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;

namespace Project_3.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetData(DateTime? start_day, DateTime? end_day)
        {
            var listData = Common.Db.RESULTS.Include(x => x.EXAMINATION).ToList();
            if (!string.IsNullOrEmpty(start_day.ToString()))
            {
                listData = listData.Where(x => x.EXAMINATION.Start_time >= start_day).ToList();
                if (!string.IsNullOrEmpty(end_day.ToString()))
                {
                    listData = listData.Where(x => x.EXAMINATION.Start_time < end_day).ToList();
                }
            }
            var data = listData.GroupBy(x => x.ExaminationId).Select(x => new
            {
                ExaminationId = x.Key,
                Name = x.FirstOrDefault().EXAMINATION.ExaminationName + " (" + x.FirstOrDefault().EXAMINATION.Start_time.ToString("dd-MM-yyyy") + ")",
                Total = x.Count()
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}