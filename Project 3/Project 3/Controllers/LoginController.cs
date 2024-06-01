using Project_3.Data;
using System.Linq;
using System.Web.Mvc;

namespace Project_3.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // Check text box null or empty
        public bool CheckNullOrEmpty(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("username", "Please enter Username.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Please enter Password.");
            }
            return true;
        }

        [HttpPost]
        // POST: Login
        public ActionResult Index(string username, string password)
        {
            if (CheckNullOrEmpty(username, password) && ModelState.IsValid)
            {
                var data = Common.Db.CANDIDATES.ToList().Where(c => c.Username.Equals(username.ToLower())).FirstOrDefault();
                if (data != null)
                {
                    if (data.Password.Equals(Common.Hash(password)))
                    {
                        Session[Candidate.candidateId] = data.CandidateId;
                        Session[Candidate.fullname] = data.FullName;
                        Session[Candidate.username] = data.Username;
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("password", "Incorrect password.");
                }
                else
                {
                    ModelState.AddModelError("username", "Username does not exist.");
                }
                return View(ModelState);
            }
            return View();
        }

        [HttpGet]
        // GET: Login/Logout
        public ActionResult Logout()
        {
            Session.Remove(Candidate.candidateId);
            Session.Remove(Candidate.fullname);
            Session.Remove(Candidate.username);
            return RedirectToAction("Index", "Home");
        }
    }
}