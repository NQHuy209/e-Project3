using Project_3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_3.Controllers
{
    public class ExamController : Controller
    {
        // GET: Exam
        public ActionResult Index()
        {
            if (Session[Candidate.candidateId] != null)
            {
                Candidate.id = int.Parse(Session[Candidate.candidateId].ToString());
            }
            Common.UpdateStatusExam();
            Common.UpdateResult();
            var data = Common.Db.EXAMINATION.ToList().Where(e => e.StatusId == 2).ToList();
            return View(data);
        }

        //POST: Exam
        [HttpPost]
        public ActionResult Index(int ExamId)
        {
            if (Common.CheckPassTheExam())
            {
                var data = Common.Db.EXAMINATION.ToList().Where(e => e.StatusId == 2).ToList();
                ViewData["Message"] = "You have already passed one exam so you don't need to take another exam.";
                return View(data);
            }
            return RedirectToAction("Topic", "AptitudeTest", new { id = ExamId });
        }
    }
}