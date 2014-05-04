using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiraclesForMito.Models;
using MiraclesForMito.Utilities;

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
			//To pass it in to _Layout.cshtml which calls Footer which calls _InterestedUserPartial
			ViewBag.InterestedUserModel = model;

			if (ModelState.IsValid)
			{
				string successMessage = string.Format("Thank you {}, we have your information and we'll reach out to you soon", model.FirstName);
				//ModelState.Add("SuccessMessage", new ModelState());
				TempData.Add("SuccessMessage", successMessage);
			}

			return View(model);
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
