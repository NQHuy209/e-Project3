using PagedList;
using Project_3.Data;
using Project_3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Project_3.Areas.Admin.Controllers
{
    public class CandidateController : BaseController
    {
        #region Index
        // GET: Admin/Candidate
        public ActionResult Index(string searchPage, string search, int? page)
        {
            var data = new List<CANDIDATES>();
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
                data = Common.Db.CANDIDATES.ToList().Where(c => c.FullName.ToLower().Contains(text) || c.Username.Contains(text)).ToList();
            }
            else
            {
                data = Common.Db.CANDIDATES.ToList();
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
        // GET: Admin/Candidate/Create
        public ActionResult Create()
        {
            return View();
        }

        public bool CheckValidateCreate(CANDIDATES candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate.Username))
            {
                ModelState.AddModelError("Username", "Please enter Username");
            }
            if (string.IsNullOrWhiteSpace(candidate.Password))
            {
                ModelState.AddModelError("Password", "Please enter Password");
            }
            if (string.IsNullOrWhiteSpace(candidate.Education))
            {
                ModelState.AddModelError("Education", "Please enter Education");
            }
            if (string.IsNullOrWhiteSpace(candidate.WorkExperience))
            {
                ModelState.AddModelError("WorkExperience", "Please enter Work Experience");
            }
            if (candidate.Birth > DateTime.Now)
            {
                ModelState.AddModelError("Birth", "Invalid date of birth.");
            }
            if (!string.IsNullOrWhiteSpace(candidate.Username) && !Regex.IsMatch(candidate.Username, "^[a-zA-Z][\\w.]{2,14}$"))
            {
                ModelState.AddModelError("Username", "Username must start with a letter and be between 3-15 characters. Usernames can contain special characters: underscores and periods.");
            }
            if (!string.IsNullOrWhiteSpace(candidate.Password) && !Regex.IsMatch(candidate.Password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$"))
            {
                ModelState.AddModelError("Password", "Password minimum eight characters, at least one uppercase letter, one lowercase letter, and one number.");
            }

            var username = Common.Db.CANDIDATES.ToList().Where(c => c.Username == candidate.Username).FirstOrDefault();
            if (username != null)
            {
                ModelState.AddModelError("Username", "Username already exists");
            }

            var email = Common.Db.CANDIDATES.ToList().Where(c => c.Email == candidate.Email).FirstOrDefault();
            if (email != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
            }

            var phoneNumber = Common.Db.CANDIDATES.ToList().Where(c => c.PhoneNumber == candidate.PhoneNumber).FirstOrDefault();
            if (phoneNumber != null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number already exists");
            }
            return true;
        }

        // POST: Admin/Candidate/Create
        [HttpPost]
        public ActionResult Create(CANDIDATES candidate)
        {
            if (CheckValidateCreate(candidate) && ModelState.IsValid)
            {
                // Add Candidate
                candidate.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(candidate.FullName.ToLower());
                candidate.Username = candidate.Username.ToLower();
                candidate.Password = Common.Hash(candidate.Password);
                Common.Db.CANDIDATES.Add(candidate);
                Common.Db.SaveChanges();
                ViewData["Message"] = "Create successful candidate";
            }
            return View(candidate);
        }
        #endregion

        #region Update
        // GET: Admin/Candidate/Update
        public ActionResult Update(int? id)
        {
            var data = Common.Db.CANDIDATES.ToList().Where(c => c.CandidateId == id).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        public bool CheckValidateUpdate(CANDIDATES candidate, int? id)
        {
            if (string.IsNullOrWhiteSpace(candidate.Education))
            {
                ModelState.AddModelError("Education", "Please enter Education");
            }
            if (string.IsNullOrWhiteSpace(candidate.WorkExperience))
            {
                ModelState.AddModelError("WorkExperience", "Please enter Work Experience");
            }
            if (candidate.Birth > DateTime.Now)
            {
                ModelState.AddModelError("Birth", "Invalid date of birth.");
            }

            var email = Common.Db.CANDIDATES.ToList().Where(c => c.Email == candidate.Email && c.CandidateId != id).FirstOrDefault();
            if (email != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
            }

            var phoneNumber = Common.Db.CANDIDATES.ToList().Where(c => c.PhoneNumber == candidate.PhoneNumber && c.CandidateId != id).FirstOrDefault();
            if (phoneNumber != null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number already exists");
            }
            return true;
        }

        // POST: Admin/Candidate/Update
        [HttpPost]
        public ActionResult Update(CANDIDATES candidate, int? id)
        {
            var data = Common.Db.CANDIDATES.ToList().Where(c => c.CandidateId == id).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            if (CheckValidateUpdate(candidate, id) && ModelState.IsValid)
            {
                data.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(candidate.FullName.ToLower());
                data.Birth = candidate.Birth;
                data.Gender = candidate.Gender;
                data.PhoneNumber = candidate.PhoneNumber;
                data.Email = candidate.Email;
                data.Address = candidate.Address;
                data.Education = candidate.Education;
                data.WorkExperience = candidate.WorkExperience;

                Common.Db.SaveChanges();
                ViewData["Message"] = "Update successful candidate";
            }
            return View(data);
        }
        #endregion

        #region History
        // GET: Admin/Candidate/History
        public ActionResult History(int? id, string searchPage, string search, int? page)
        {
            var checkCandidate = Common.Db.CANDIDATES.ToList().Where(c => c.CandidateId == id).FirstOrDefault();
            if (checkCandidate == null)
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
                data = Common.Db.RESULTS.Include(r => r.STATUS_RESULTS).Include(r => r.CANDIDATES).Include(r => r.EXAMINATION).ToList()
                    .Where(r => r.CandidateId == id && r.EXAMINATION.ExaminationName.ToLower().Contains(text)).ToList();
            }
            else
            {
                data = Common.Db.RESULTS.Include(r => r.STATUS_RESULTS).Include(r => r.CANDIDATES).Include(r => r.EXAMINATION).ToList().Where(r => r.CandidateId == id).ToList();
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            ViewData["Search"] = search;
            ViewData["Username"] = checkCandidate.Username;
            ViewData["Page"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region ResetPassword
        // GET: Admin/Candidate/ResetPassword
        public ActionResult ResetPassword(int? id)
        {
            var data = Common.Db.CANDIDATES.ToList().Where(c => c.CandidateId == id).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        // POST: Admin/Candidate/ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string pass, int? id)
        {
            var data = Common.Db.CANDIDATES.ToList().Where(c => c.CandidateId == id).FirstOrDefault();
            if (data == null)
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