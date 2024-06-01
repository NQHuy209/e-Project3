using PagedList;
using Project_3.Data;
using Project_3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Project_3.Areas.Admin.Controllers
{
    public class ExamController : BaseController
    {
        public List<EXAMINATION> ExamManagerList()
        {
            return Common.Db.EXAMINATION.Include(e => e.STATUS_EXAM).ToList().Where(e => e.REL_MANAGER_EXAMINATION.Any(x => x.ManagerId == Manager.id)).OrderByDescending(e => e.ExaminationId).ToList();
        }

        private bool CheckTime(EXAMINATION examination)
        {
            if (examination.Start_time < DateTime.Now.AddMinutes(10))
            {
                ModelState.AddModelError("Start_time", "The start time must be 10 minutes greater than the current time.");
            }
            if (examination.End_time < examination.Start_time.AddHours(2))
            {
                ModelState.AddModelError("End_time", "The end time must always be 2 hours greater than the start time.");
            }
            return true;
        }

        #region Index
        // GET: Admin/Exam
        [HttpGet]
        public ActionResult Index(string searchPage, string search, int? statusPage, int? status, int? page)
        {
            var data = new List<EXAMINATION>();
            if (status != null || search != null)
            {
                page = 1;
            }
            else
            {
                status = statusPage;
                search = searchPage;
            }
            if (!string.IsNullOrEmpty(search))
            {
                string text = search.ToLower();
                if (status == null || status == 0)
                {
                    data = ExamManagerList().Where(e => e.ExaminationName.ToLower().Contains(text)).ToList();
                }
                else
                {
                    data = ExamManagerList().Where(e => e.StatusId == status && e.ExaminationName.ToLower().Contains(text)).ToList();
                }
            }
            else
            {
                if (status == null || status == 0)
                {
                    data = ExamManagerList();
                }
                else
                {
                    data = ExamManagerList().Where(e => e.StatusId == status).ToList();
                }
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["Status"] = status;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region Create
        // GET: Admin/Exam/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Exam/Create
        [HttpPost]
        public ActionResult Create(EXAMINATION examination)
        {
            if (CheckTime(examination) && ModelState.IsValid)
            {
                // Add Examnination
                examination.Score = 0;
                examination.StatusId = 1;
                Common.Db.EXAMINATION.Add(examination);
                Common.Db.SaveChanges();

                // Add Manager
                REL_MANAGER_EXAMINATION manager_exam = new REL_MANAGER_EXAMINATION();
                manager_exam.ManagerId = Manager.id;
                manager_exam.ExaminationId = Common.Db.EXAMINATION.ToList().Last().ExaminationId;
                manager_exam.ManagerId_create = Manager.id;
                Common.Db.REL_MANAGER_EXAMINATION.Add(manager_exam);
                Common.Db.SaveChanges();
                ViewData["Message"] = "Create successful examination";
            }
            return View(examination);
        }
        #endregion

        #region Update
        // GET: Admin/Exam/Update/id
        [HttpGet]
        public ActionResult Update(int? id)
        {
            var data = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        // POST: Admin/Exam/Update/id
        [HttpPost]
        public ActionResult Update(EXAMINATION examination, int? id)
        {
            var data = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            if (CheckTime(examination) && ModelState.IsValid)
            {
                data.ExaminationName = examination.ExaminationName;
                data.Start_time = examination.Start_time;
                data.End_time = examination.End_time;
                data.Descriptions = examination.Descriptions;
                Common.Db.SaveChanges();
                ViewData["Message"] = "Update successful examination";
            }
            return View(data);
        }
        #endregion

        #region Details
        // GET: Admin/Exam/Details/id
        [HttpGet]
        public ActionResult Details(int? id, int? topic)
        {
            var data = ExamManagerList().Where(e => e.ExaminationId == id).FirstOrDefault();
            if (data == null || topic == null)
            {
                return HttpNotFound();
            }

            var dataQuestion = Common.Db.REL_EXAMINATION_QUESTIONS.Include(x => x.QUESTIONS).ToList().Where(x => x.EXAMINATION.ExaminationId == id && x.TopicId == topic).ToList();
            ViewData["Status"] = data.StatusId;
            ViewData["Topic"] = topic;
            ViewData["QuestionNumber"] = dataQuestion.Count();
            ViewData["Score"] = dataQuestion.Sum(x => x.QUESTIONS.Point);
            return View(dataQuestion);
        }

        // GET: Admin/Exam/Details/id
        [HttpPost]
        public ActionResult Details(REL_EXAMINATION_QUESTIONS input, int? id)
        {
            var data = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }

            var dataQuestionInExam = Common.Db.REL_EXAMINATION_QUESTIONS.ToList().Where(x => x.ExaminationId == id && x.QuestionId == input.QuestionId).FirstOrDefault();
            if (dataQuestionInExam != null)
            {
                Common.Db.REL_EXAMINATION_QUESTIONS.Remove(dataQuestionInExam);
                Common.Db.SaveChanges();

                var total = 0;
                var checkQuestionInExam = Common.Db.REL_EXAMINATION_QUESTIONS.ToList().Any(x => x.ExaminationId == id);
                if (checkQuestionInExam)
                {
                    total = Common.Db.REL_EXAMINATION_QUESTIONS.ToList().Where(x => x.ExaminationId == id).Sum(x => x.QUESTIONS.Point);
                }
                data.Score = total;
                Common.Db.SaveChanges();
            }
            return RedirectToAction("Details", "Exam", new { topic = Common.SearchTopic });
        }
        #endregion

        #region Add Question to Exam
        // GET: Admin/Exam/AddQuestionToExam/id
        [HttpGet]
        public ActionResult AddQuestionToExam(int? id, string searchPage, string search, int? topicPage, int? topic, int? page)
        {
            var checkExam = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (checkExam == null)
            {
                return HttpNotFound();
            }

            var data = new List<QUESTIONS>();
            var listQuestion = Common.Db.QUESTIONS.Include(q => q.TOPIC).Include(q => q.CORRECT_ANSWER).ToList();
            if (topic != null || search != null)
            {
                page = 1;
            }
            else
            {
                topic = topicPage;
                search = searchPage;
            }
            if (!string.IsNullOrEmpty(search))
            {
                string text = search.ToLower();
                if (topic == null || topic == 0)
                {
                    data = listQuestion.Where(q => q.Content.ToLower().Contains(text)).ToList();
                }
                else
                {
                    data = listQuestion.Where(q => q.TOPIC.TopicId == topic && q.Content.ToLower().Contains(text)).ToList();
                }
            }
            else
            {
                if (topic == null || topic == 0)
                {
                    data = listQuestion;
                }
                else
                {
                    data = listQuestion.Where(q => q.TOPIC.TopicId == topic).ToList();
                }
            }
            foreach (var item in Common.Topic())
            {
                int numberQuestionTopic = Common.Db.REL_EXAMINATION_QUESTIONS.ToList().Where(x => x.ExaminationId == id && x.TopicId == item.TopicId).Count();
                ViewData[item.TopicName] = item.TopicName + ": " + numberQuestionTopic + "/" + item.Question_number;
            }
            ViewData["ExamId"] = checkExam.ExaminationId;
            ViewData["ExamName"] = checkExam.ExaminationName;

            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["Topic"] = topic;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public static bool CheckQuestionInExam(REL_EXAMINATION_QUESTIONS input, int? id)
        {
            foreach (var item in Common.Topic())
            {
                int numberQuestionTopic = Common.Db.REL_EXAMINATION_QUESTIONS.ToList().Where(x => x.ExaminationId == id && x.TopicId == item.TopicId).Count();
                if (input.TopicId == item.TopicId && numberQuestionTopic >= item.Question_number)
                {
                    Common.Fail = "There are enough " + item.TopicName + " questions in the exam.";
                    return false;
                }
            }

            // Check if the question already exists in the exam
            var checkQuestionExist = Common.Db.REL_EXAMINATION_QUESTIONS.ToList().Any(x => x.ExaminationId == id && x.QuestionId == input.QuestionId);
            if (checkQuestionExist)
            {
                Common.Fail = "This question already exists in the exam.";
                return false;
            }
            return true;
        }

        // POST: Admin/Exam/AddQuestionToExam/id
        [HttpPost]
        public ActionResult AddQuestionToExam(REL_EXAMINATION_QUESTIONS input, int? id)
        {
            var data = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }

            if (CheckQuestionInExam(input, id))
            {
                input.ManagerId = Manager.id;
                Common.Db.REL_EXAMINATION_QUESTIONS.Add(input);
                Common.Db.SaveChanges();

                var total = Common.Db.REL_EXAMINATION_QUESTIONS.ToList().Where(x => x.ExaminationId == id).Sum(x => x.QUESTIONS.Point);
                data.Score = total;
                Common.Db.SaveChanges();
            }
            return RedirectToAction("AddQuestionToExam", "Exam", new { page = Common.Page, searchPage = Common.Search, topicPage = Common.SearchTopic });
        }
        #endregion

        #region Manager Exam
        // GET: Admin/Exam/ManagerExam/id
        [HttpGet]
        public ActionResult ManagerExam(int? id, string searchPage, string search, int? page)
        {
            var checkExam = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (checkExam == null || !Common.ManagerCreateExam(id, Manager.id))
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
                data = Common.Db.MANAGER.ToList().Where(m => m.REL_MANAGER_EXAMINATION.Any(x => x.ExaminationId == id) && (m.FullName.ToLower().Contains(text) || m.Username.ToLower().Contains(text))).ToList();
            }
            else
            {
                data = Common.Db.MANAGER.ToList().Where(m => m.REL_MANAGER_EXAMINATION.Any(x => x.ExaminationId == id)).ToList();
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["ExamId"] = id;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        // POST: Admin/Exam/ManagerExam/id
        [HttpPost]
        public ActionResult ManagerExam(REL_MANAGER_EXAMINATION input, int? id)
        {
            var checkExam = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (checkExam == null || !Common.ManagerCreateExam(id, Manager.id))
            {
                return HttpNotFound();
            }

            var data = Common.Db.REL_MANAGER_EXAMINATION.ToList().Where(x => x.ExaminationId == id && x.ManagerId == input.ManagerId).FirstOrDefault();
            if (data != null && !Common.ManagerCreateExam(id, input.ManagerId))
            {
                Common.Db.REL_MANAGER_EXAMINATION.Remove(data);
                Common.Db.SaveChanges();
            }
            return RedirectToAction("ManagerExam", "Exam", new { page = Common.Page, searchPage = Common.Search });
        }
        #endregion

        #region Add Manager to Exam
        // GET: Admin/Exam/AddManagerToExam/id
        [HttpGet]
        public ActionResult AddManagerToExam(int? id, int? page, string searchPage, string search)
        {
            var checkExam = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (checkExam == null || !Common.ManagerCreateExam(id, Manager.id))
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
                data = Common.Db.MANAGER.ToList().Where(m => m.FullName.ToLower().Contains(text) || m.Username.ToLower().Contains(text)).ToList();
            }
            else
            {
                data = Common.Db.MANAGER.ToList();
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["ExamId"] = id;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult AddManagerToExam(REL_MANAGER_EXAMINATION input, int? id)
        {
            var checkExam = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (checkExam == null || !Common.ManagerCreateExam(id, Manager.id))
            {
                return HttpNotFound();
            }

            var checkManager = Common.Db.REL_MANAGER_EXAMINATION.ToList().Any(x => x.ExaminationId == input.ExaminationId && x.ManagerId == input.ManagerId);
            if (checkManager)
            {
                Common.Fail = checkExam.ExaminationName + " already exists this management.";
                return RedirectToAction("AddManagerToExam", "Exam");
            }

            Common.Db.REL_MANAGER_EXAMINATION.Add(input);
            Common.Db.SaveChanges();
            Common.Success = "Add " + input.MANAGER.FullName + " to " + checkExam.ExaminationName + " successfully.";
            return RedirectToAction("AddManagerToExam", "Exam", new { page = Common.Page, searchPage = Common.Search });
        }
        #endregion

        #region Activate
        // GET: Admin/Exam/Activate/id
        public ActionResult Activate(int? id)
        {
            var data = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId == 1).FirstOrDefault();
            if (data != null)
            {
                // Check number question in exam
                var numberQuestion = Common.Db.REL_EXAMINATION_QUESTIONS.ToList().Where(x => x.ExaminationId == id).Count();
                if (numberQuestion == 25)
                {
                    if (data.Start_time <= DateTime.Now)
                    {
                        Common.Fail = "Please edit the appropriate time for activation.";
                        return RedirectToAction("", "Exam", new { page = Common.Page, searchPage = Common.Search, statusPage = Common.SearchStatus });
                    }
                    // Can only be activated when 25 questions are reached
                    data.StatusId = 2;
                    Common.Db.SaveChanges();
                    Common.Success = "Activate " + data.ExaminationName + " successfully.";
                }
                else
                {
                    Common.Fail = "Cannot activate because the number of questions is not enough.";
                }
            }
            return RedirectToAction("", "Exam", new { page = Common.Page, searchPage = Common.Search, statusPage = Common.SearchStatus });
        }
        #endregion

        #region Result
        // GET: Admin/Exam/Result/id
        [HttpGet]
        public ActionResult Result(int? id, string searchPage, string search, int? page)
        {
            var checkExam = ExamManagerList().Where(e => e.ExaminationId == id && e.StatusId != 1).FirstOrDefault();
            if (checkExam == null)
            {
                return HttpNotFound();
            }
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
                data = Common.Db.RESULTS.Include(r => r.STATUS_RESULTS).Include(r => r.EXAMINATION).Include(r => r.CANDIDATES).ToList()
                    .Where(r => r.ExaminationId == id && (r.CANDIDATES.FullName.ToLower().Contains(text) || r.CANDIDATES.Username.ToLower().Contains(text))).ToList();
            }
            else
            {
                data = Common.Db.RESULTS.Include(r => r.STATUS_RESULTS).Include(r => r.EXAMINATION).Include(r => r.CANDIDATES).ToList()
                    .Where(r => r.ExaminationId == id).ToList();
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["Search"] = search;
            ViewData["ExamName"] = checkExam.ExaminationName;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        #endregion
    }
}