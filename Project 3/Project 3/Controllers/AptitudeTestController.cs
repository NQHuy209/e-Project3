using Project_3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using Project_3.Models;

namespace Project_3.Controllers
{
    public class AptitudeTestController : BaseController
    {
        #region Index
        // GET: AptitudeTest
        public ActionResult Index(int? id)
        {
            // Kiểm tra trạng thái kỳ thi này có đang hoạt động và thời gian phù hợp không, nếu không thì không cho thi.
            var checkExam = Common.Db.EXAMINATION.ToList().Where(e => e.ExaminationId == id && e.StatusId == 2 && e.Start_time <= DateTime.Now && e.End_time >= DateTime.Now).FirstOrDefault();
            if (checkExam == null)
            {
                return RedirectToAction("", "Exam");
            }

            // Kiểm tra xem có kết quả Pass hay Fail chưa, có rồi thì không cho thi kỳ thi này nữa.
            var checkResult = Common.Db.RESULTS.ToList().Any(r => r.CandidateId == Candidate.id && r.ExaminationId == id && (r.StatusId == 2 || r.StatusId == 3));
            if (checkResult)
            {
                return RedirectToAction("", "Exam");
            }

            int topicId = 1;
            // Kiểm tra xem thí sinh có đang tham gia kỳ thi không
            var checkStatusTest = Common.Db.RESULTS.ToList().Where(r => r.CandidateId == Candidate.id && r.ExaminationId == id).FirstOrDefault();
            if (checkStatusTest == null)
            {
                // Nếu không có thì bắt đầu mới.
                RESULTS result = new RESULTS();
                result.CandidateId = Candidate.id;
                result.ExaminationId = (int)id;
                result.Score = 0;
                result.TopicId = 1;
                result.StatusId = 1;
                Common.Db.RESULTS.Add(result);
            }
            else
            {
                // Nếu có thì kiểm tra đang làm chủ đề mấy.
                var topicTest = Common.Db.RESULTS.ToList().Where(r => r.CandidateId == Candidate.id && r.ExaminationId == id).FirstOrDefault().TopicId;
                if (topicTest == 1)
                {
                    // Nếu chủ đề đang làm là 1 mà load lại trang thì sẽ lấy chủ đề 2
                    topicId = 2;
                }
                if (topicTest == 2)
                {
                    // Nếu chủ đề đang làm là 2 mà load lại trang thì sẽ lấy chủ đề 3
                    topicId = 3;
                }
                if (topicTest == 3)
                {
                    // Nếu chủ đề đang làm là 3 mà load lại trang thì sẽ trả về 0 và kết thúc bài thi
                    topicId = 0;
                }
                checkStatusTest.TopicId = topicId;
            }
            Common.Db.SaveChanges();

            if (topicId == 0)
            {
                return RedirectToAction("Result", "AptitudeTest", new { @id = id });
            }

            var topic = Common.Db.TOPIC.ToList().Where(t => t.TopicId == topicId).FirstOrDefault();
            var data = Common.Db.REL_EXAMINATION_QUESTIONS.Include(x => x.EXAMINATION).Include(x => x.QUESTIONS.TOPIC).ToList()
                .Where(x => x.ExaminationId == id && x.TopicId == topicId).ToList();

            ViewData["ExamId"] = id;
            ViewData["ExamName"] = checkExam.ExaminationName;
            ViewData["TopicId"] = topicId;
            ViewData["Topic"] = topic.TopicName;
            ViewData["QuestionNumber"] = topic.Question_number;
            ViewData["min"] = topic.Time_to_do;
            return View(data);
        }

        // POST: AptitudeTest
        [HttpPost]
        public ActionResult Index(FormCollection form, int[] questionId, int? id)
        {
            // Kiểm tra xem có kết quả Pass hay Fail chưa, có rồi thì không cho thi kỳ thi này nữa.
            var checkResult = Common.Db.RESULTS.ToList().Any(r => r.CandidateId == Candidate.id && r.ExaminationId == id && (r.StatusId == 2 || r.StatusId == 3));
            if (checkResult)
            {
                return RedirectToAction("", "Exam");
            }

            int topicId = Common.Db.RESULTS.ToList().Where(r => r.CandidateId == Candidate.id && r.ExaminationId == id).FirstOrDefault().TopicId;
            var count = Common.Db.TOPIC.ToList().Where(t => t.TopicId == topicId).FirstOrDefault().Question_number;
            for (int i = 0; i < count; i++)
            {
                if (form["Question" + i] != null)
                {
                    CANDIDATES_TEST test = new CANDIDATES_TEST();
                    test.CandidateId = Candidate.id;
                    test.ExaminationId = (int)id;
                    test.QuestionId = questionId[i];
                    test.TopicId = topicId;
                    test.Candidate_Answer = int.Parse(form["Question" + i].ToString());
                    Common.Db.CANDIDATES_TEST.Add(test);
                    Common.Db.SaveChanges();
                }
            }

            var score = Common.Db.CANDIDATES_TEST.Include(ct => ct.QUESTIONS).ToList()
                .Where(ct => ct.CandidateId == Candidate.id && ct.ExaminationId == id && ct.Candidate_Answer == ct.QUESTIONS.Correct_Answer_Id)
                .Sum(ct => ct.QUESTIONS.Point);

            var result = Common.Db.RESULTS.ToList().Where(r => r.CandidateId == Candidate.id && r.ExaminationId == id).FirstOrDefault();
            result.Score = score;
            Common.Db.SaveChanges();
            if (topicId == 3)
            {
                result.TopicId = 0;
                Common.Db.SaveChanges();
                return RedirectToAction("Result", "AptitudeTest", new { @id = id });
            }
            return RedirectToAction("Index", "AptitudeTest", new { @id = id });
        }
        #endregion

        // GET: AptitudeTest/Result
        public ActionResult Result(int? id)
        {
            var result = Common.Db.RESULTS.ToList().Where(r => r.CandidateId == Candidate.id && r.ExaminationId == id && r.TopicId == 0 && r.StatusId == 1).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("", "Exam");
            }
            var scoreExam = Common.Db.EXAMINATION.ToList().Where(e => e.ExaminationId == id).FirstOrDefault().Score;
            float scorePass = scoreExam * 0.6f;
            if (result.Score > scorePass)
            {
                ViewData["Message"] = "You have cleared this round, next round would be HR Round.";
                result.StatusId = 2;
            }
            else
            {
                ViewData["Message"] = "Thank you for completing the test.";
                result.StatusId = 3;
            }
            Common.Db.SaveChanges();
            ViewData["ExamId"] = id;
            return View();
        }

        // GET: AptitudeTest/Topic
        public ActionResult Topic(int? id)
        {
            var data = Common.Db.EXAMINATION.ToList().Where(e => e.ExaminationId == id && e.StatusId == 2 && e.Start_time <= DateTime.Now && e.End_time >= DateTime.Now).FirstOrDefault();
            if (data == null)
            {
                return RedirectToAction("", "Exam");
            }
            ViewData["ExamId"] = id;
            return View(data);
        }

        // GET: AptitudeTest/Guide
        public ActionResult Guide(int? id)
        {
            var data = Common.Db.EXAMINATION.ToList().Where(e => e.ExaminationId == id && e.StatusId == 2 && e.Start_time <= DateTime.Now && e.End_time >= DateTime.Now).FirstOrDefault();
            if (data == null)
            {
                return RedirectToAction("", "Exam");
            }

            var topic = Common.Db.TOPIC.ToList();
            ViewData["Time"] = topic.Sum(t => t.Time_to_do);
            ViewData["QuestionNumber"] = topic.Sum(t => t.Question_number);
            ViewData["ExamId"] = id;
            return View(data);
        }
    }
}