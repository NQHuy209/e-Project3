using Project_3.Data;
using System;
using System.Web.Mvc;

namespace Project_3.Areas.Admin.Controllers
{
	public class BaseController : Controller
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			Common.UpdateStatusExam();
			Common.UpdateResult();
			if (Session[Manager.managerId] == null)
			{
				filterContext.Result = new RedirectToRouteResult(
					new System.Web.Routing.RouteValueDictionary(new { Controller = "Login" })
				);
			}
			else
			{
				Manager.id = int.Parse(Session[Manager.managerId].ToString());
				string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
				bool flag1 = controller.Equals("Role") && !Common.CheckRole(1);
				bool flag2 = controller.Equals("Manager") && !Common.CheckRole(2);
				bool flag3 = controller.Equals("Candidate") && !Common.CheckRole(3);
				if (flag1 || flag2 || flag3)
				{
					filterContext.Result = new RedirectToRouteResult(
						new System.Web.Routing.RouteValueDictionary(new { Controller = "" })
					);
				}
			}
			base.OnActionExecuting(filterContext);
		}
	}
}