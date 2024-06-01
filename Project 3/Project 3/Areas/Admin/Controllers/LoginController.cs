using Project_3.Data;
using System.Linq;
using System.Web.Mvc;

namespace Project_3.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            if (Session[Manager.managerId] != null)
            {
                return RedirectToAction("", "Home");
            }
            return View();
        }

        // Check text box null or empty
        public bool CheckNullOrEmpty(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "Please enter Username.");
            }
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "Please enter Password.");
            }
            return true;
        }

        [HttpPost]
        // POST: Admin/Login
        public ActionResult Index(string username, string password)
        {
            if (CheckNullOrEmpty(username, password) && ModelState.IsValid)
            {
                var data = Common.Db.MANAGER.ToList().Where(c => c.Username.Equals(username.ToLower())).FirstOrDefault();
                if (data != null)
                {
                    if (data.Password.Equals(Common.Hash(password)))
                    {
                        Session[Manager.managerId] = data.ManagerId;
                        Session[Manager.fullname] = data.FullName;
                        Session[Manager.username] = data.Username;
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
        // GET: Admin/Login/Logout
        public ActionResult Logout()
        {
            Session.Remove(Manager.managerId);
            Session.Remove(Manager.fullname);
            Session.Remove(Manager.username);
            return RedirectToAction("Index", "Home");
        }
    }
}