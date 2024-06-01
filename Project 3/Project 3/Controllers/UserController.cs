using Project_3.Data;
using Project_3.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Project_3.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            var data = Common.Db.CANDIDATES.ToList().FirstOrDefault(c => c.CandidateId == Candidate.id);
            if (data == null )
            {
                return HttpNotFound();
            }
            return View(data);
        }

        #region Update
        // GET: User/Update
        public ActionResult Update()
        {
            var data = Common.Db.CANDIDATES.ToList().FirstOrDefault(c => c.CandidateId == Candidate.id);
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }

        private bool CheckValidateUpdate(CANDIDATES candidate)
        {
            if (candidate.Birth > DateTime.Now)
            {
                ModelState.AddModelError("Birth", "Invalid date of birth.");
            }

            var email = Common.Db.CANDIDATES.ToList().Where(c => c.Email == candidate.Email && c.CandidateId != Candidate.id).FirstOrDefault();
            if (email != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
            }

            var phoneNumber = Common.Db.CANDIDATES.ToList().Where(c => c.PhoneNumber == candidate.PhoneNumber && c.CandidateId != Candidate.id).FirstOrDefault();
            if (phoneNumber != null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number already exists");
            }
            return true;
        }

        [HttpPost]
        // POST: User/Update
        public ActionResult Update(CANDIDATES candidate)
        {
            var data = Common.Db.CANDIDATES.ToList().Where(c => c.CandidateId == Candidate.id).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            if (CheckValidateUpdate(candidate) && ModelState.IsValid)
            {
                data.FullName = candidate.FullName;
                data.Birth = candidate.Birth;
                data.Gender = candidate.Gender;
                data.PhoneNumber = candidate.PhoneNumber;
                data.Email = candidate.Email;
                data.Address = candidate.Address;

                Common.Db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }
        #endregion

        #region Change Password
        // GET: User/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        private bool CheckChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Please enter Current Password.");
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ModelState.AddModelError("NewPassword", "Please enter New Password.");
            }
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "Please enter Confirm Password.");
            }
            if (!Common.Hash(CurrentPassword).Equals(password))
            {
                ModelState.AddModelError("CurrentPassword", "Incorrect password.");
            }
            if (CurrentPassword.Equals(NewPassword))
            {
                ModelState.AddModelError("NewPassword", "The new password must not be the same as the current password.");
            }
            if (!Regex.IsMatch(NewPassword, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$"))
            {
                ModelState.AddModelError("NewPassword", "Password minimum eight characters, at least one uppercase letter, one lowercase letter, and one number.");
            }
            if (!NewPassword.Equals(ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "Confirmation password is incorrect.");
            }
            return true;
        }

        [HttpPost]
        // POST: User/ChangePassword
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var data = Common.Db.CANDIDATES.ToList().Where(c => c.CandidateId == Candidate.id).FirstOrDefault();
            if (data == null)
            {
                return HttpNotFound();
            }
            if (CheckChangePassword(CurrentPassword, NewPassword, ConfirmPassword, data.Password) && ModelState.IsValid)
            {
                data.Password = Common.Hash(NewPassword);
                Common.Db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        #endregion
    }
}