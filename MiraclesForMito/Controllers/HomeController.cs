using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiraclesForMito.Models;
using MiraclesForMito.Utilities;
using System.Net.Mail;
using System.IO;

namespace MiraclesForMito.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Index(InterestedUserModel model)
		{
			if (ModelState.IsValid)
			{
				//Have the information we need, now try to send a mesage
				//TODO: Get a better email address?
				string sendTo = EmailHelpers.SEND_EMAIL_ADDRESS;
				string body;
				using (StringWriter sw = new StringWriter()){
					//http://stackoverflow.com/a/2759898/1175496
					ViewData.Model = model;
					var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "~/Views/Emails/_InterestedUserInfoPartial.cshtml");
					var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
					viewResult.View.Render(viewContext, sw);
					viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
					body = sw.GetStringBuilder().ToString();
				}

				MailMessage msg = new MailMessage(EmailHelpers.SEND_EMAIL_ADDRESS, sendTo)
				{
					Subject = "Miracles for Mito Interested Person",
					Body = body,
					IsBodyHtml = true
				};

				InterestedSuccessViewModel viewModel = new InterestedSuccessViewModel{
					InterestedUserModel = model,
					SendTo = sendTo,
					EmailMessage = msg,
					SuccessfullySentEmail = EmailHelpers.SendEmail(msg),
				};

				return View("InterestedSuccess", viewModel);
			}
			else
			{
				//Modern browsers will validate client-side, but if not, re-populate the form and ValidationSummary() explains the problems
				ViewBag.InterestedUserModel = model;
				return View(model);
			}

		}

		public ActionResult Unsubscribe()
		{
			var id = RouteData.Values["id"];

			if (id != null)
			{
				// we encrypted the email in the url so we need to grab it then decrypt it
				string sEncryptedEmail = id.ToString();
				string sDecrypted = SimpleCrypto.Decrypt(sEncryptedEmail) ?? "";

				SiteDB db = new SiteDB();

				var itemToDelete = db.NotifiedList.FirstOrDefault(item => !string.IsNullOrEmpty(item.Email) && item.Email.ToLower() == sDecrypted.ToLower());

				// we try to delete the item if its there.
				if (itemToDelete != null)
				{
					db.NotifiedList.Remove(itemToDelete);
					db.SaveChanges();
				}
			}

			// no matter what we return a success. The user doesn't care if the unsubscribe doesn't work.
			return View();
		}
	}
}
