using Project_3.Data;
using System;
using System.Web.Mvc;

namespace Project_3.Controllers
{
	public class BaseController : Controller
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (Session[Candidate.candidateId] == null)
			{
				filterContext.Result = new RedirectToRouteResult(
					new System.Web.Routing.RouteValueDictionary(new { Controller = "Login" })
				);
			}
			else
			{
				Candidate.id = int.Parse(Session[Candidate.candidateId].ToString());
				string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
				if (controller.Equals("AptitudeTest") && Common.CheckPassTheExam())
				{
					filterContext.Result = new RedirectToRouteResult(
						new System.Web.Routing.RouteValueDictionary(new { Controller = "Exam" })
					);
				}
			}
			base.OnActionExecuting(filterContext);
		}
	}
}