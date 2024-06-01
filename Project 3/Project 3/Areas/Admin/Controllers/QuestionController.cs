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
    public class QuestionController : BaseController
    {
        #region Index
        // GET: Admin/Question
        public ActionResult Index(string searchPage, string search, int? topicPage, int? topic, int? page)
        {
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
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["Topic"] = topic;
            ViewData["Search"] = search;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region Create
        // GET: Admin/Question/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Question/Create
        [HttpPost]
        public ActionResult Create(QUESTIONS question)
        {
            if (ModelState.IsValid)
            {
                // Add Question
                Common.Db.QUESTIONS.Add(question);
                Common.Db.SaveChanges();

                ViewData["Message"] = "Create successful question";
            }
            return View(question);
        }
        #endregion

        #region Update
        // GET: Admin/Question/Update
        public ActionResult Update(int? id)
        {
            var data = Common.Db.QUESTIONS.ToList().Where(q => q.QuestionId == id).FirstOrDefault();
            if (data == null || Common.CheckQuestionInExamActivate(id))
            {
                return HttpNotFound();
            }
            return View(data);
        }

        // POST: Admin/Question/Update
        [HttpPost]
        public ActionResult Update(QUESTIONS question, int? id)
        {
            var data = Common.Db.QUESTIONS.ToList().Where(q => q.QuestionId == id).FirstOrDefault();
            if (data == null || Common.CheckQuestionInExamActivate(id))
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                // Update Question
                data.Content = question.Content;
                data.Answer_A = question.Answer_A;
                data.Answer_B = question.Answer_B;
                data.Answer_C = question.Answer_C;
                data.Answer_D = question.Answer_D;
                data.Point = question.Point;
                data.Correct_Answer_Id = question.Correct_Answer_Id;
                Common.Db.SaveChanges();

                ViewData["Message"] = "Update successful question";
            }
            return View(data);
        }
        #endregion
    }
}