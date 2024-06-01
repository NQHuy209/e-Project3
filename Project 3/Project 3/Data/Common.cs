using Project_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;

namespace Project_3.Data
{
	public class Common
	{
		public static Project3Entities Db = new Project3Entities();

		public static string Success;
		
		public static string Fail;

		public static int Page;

		public static string Search;

		public static int? SearchTopic;

		public static int? SearchStatus;
		
		public static string Hash(string text)
		{
			MD5 md5 = MD5.Create();
			byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
			StringBuilder hashSb = new StringBuilder();
			foreach (byte b in hash)
			{
				hashSb.Append(b.ToString("X2"));
			}
			return hashSb.ToString();
		}

		public static bool CheckRole(int roleId)
		{
			// Layout
			return Db.REL_MANAGER_ROLES.ToList().Any(x => x.ManagerId == Manager.id && x.RoleId == roleId);
		}

		public static bool CheckRoleAdmin(int? managerId)
		{
			return Db.REL_MANAGER_ROLES.ToList().Any(x => x.ManagerId == managerId && x.RoleId == 1);
		}

		public static bool CheckRoleExist(int roleId, int managerId)
		{
			return Db.REL_MANAGER_ROLES.ToList().Any(x => x.ManagerId == managerId && x.RoleId == roleId);
		}

		public static bool ManagerCreateExam(int? examId, int managerId)
		{
			// View ManagerExam (Exam)
			return Db.REL_MANAGER_EXAMINATION.ToList().Any(x => x.ExaminationId == examId && x.ManagerId == managerId && x.ManagerId_create.HasValue);
		}

		public static bool CheckManagerExam(int examId, int managerId)
		{
			// View AddManagerToExam (Exam)
			return Db.REL_MANAGER_EXAMINATION.ToList().Any(x => x.ExaminationId == examId && x.ManagerId == managerId);
		}

		public static bool CheckQuestionInExam(int examId, int questionId)
		{
			// View AddQuestionToExam (Exam)
			return Db.REL_EXAMINATION_QUESTIONS.ToList().Any(x => x.ExaminationId == examId && x.QuestionId == questionId);
		}

		public static bool CheckQuestionInExamActivate(int? questionId)
		{
			// View Index (Question)
			return Db.REL_EXAMINATION_QUESTIONS.ToList().Any(x => x.QuestionId == questionId && x.EXAMINATION.StatusId == 2);
		}

		public static bool CheckExamActivate(int examId)
		{
			return Db.EXAMINATION.ToList().Any(e => e.ExaminationId == examId && e.StatusId == 2 && e.Start_time <= DateTime.Now && e.End_time >= DateTime.Now);
		}

		public static bool CheckPassTheExam()
		{
			return Db.RESULTS.ToList().Any(r => r.CandidateId == Candidate.id && r.StatusId == 2);
		}
		
		public static List<STATUS_EXAM> StatusExam()
		{
			return Db.STATUS_EXAM.ToList();
		}
		
		public static List<TOPIC> Topic()
		{
			return Db.TOPIC.ToList();
		}
		
		public static List<CORRECT_ANSWER> CorrectAnswer()
		{
			return Db.CORRECT_ANSWER.ToList();
		}

		public static void UpdateStatusExam()
		{
			var exam = Db.EXAMINATION.ToList().Where(e => e.StatusId == 2 && e.End_time <= DateTime.Now).ToList();
			if (exam.Any())
			{
				foreach (var e in exam)
				{
					e.StatusId = 3;
					Db.SaveChanges();
				}
			}
		}

		public static void UpdateResult()
		{
			var result = Db.RESULTS.Include(r => r.EXAMINATION).ToList().Where(r => r.StatusId == 1 && r.EXAMINATION.StatusId == 3).ToList();
			if (result.Any())
			{
				foreach (var r in result)
				{
					var scoreExam = Db.EXAMINATION.ToList().Where(e => e.ExaminationId == r.ExaminationId).FirstOrDefault().Score;
					float scorePass = scoreExam * 0.6f;
					r.TopicId = 0;
					if (r.Score > scorePass)
					{
						r.StatusId = 2;
					}
					else
					{
						r.StatusId = 3;
					}
					Db.SaveChanges();
				}
			}
		}
	}
}