using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiraclesForMito.Models;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Net.Http;
using System.Net;

namespace MiraclesForMito.Controllers
{
    public class BlogController : Controller
    {
		SiteDB db = new SiteDB();

        public ActionResult Index(BlogPaginationModel model)
        {
			model.AJAXUrl = model.AJAXUrl ?? "~/Blog/Paginate";
			return View(model);
        }

		public ActionResult Paginate(PaginationModel model)
		{
			return PartialView(
				"~/Views/Blog/BlogPostsPaginationBody.cshtml",
				new BlogPaginationModel(db, model.PageIndex, model.PageSize)
			);
		}

		public ActionResult Post()
		{
			string sPostTitle = RouteData.Values["id"].ToString();
			return View(new BlogPaginationModel(db, BlogFilterType.SEOTitle, sPostTitle));
		}

		public ActionResult Author()
		{
			string sAuthorName = RouteData.Values["id"].ToString();
			var model = new BlogPaginationModel(db, BlogFilterType.Author, sAuthorName);
			model.AJAXUrl = "~/Blog/AuthorPaginate";
			return View("Index", model);
		}

		public ActionResult AuthorPaginate(PaginationModel model)
		{
			// deserialize the data
			var additionalData = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.AdditionalData);

			// cast the value to the event type we want
			BlogFilterType filterType = (BlogFilterType)int.Parse(additionalData["FilterType"]);
			string filterValue = additionalData["FilterValue"];

			return PartialView(
				"~/Views/Blog/BlogPostsPaginationBody.cshtml",
				new BlogPaginationModel(db, filterType, filterValue, model.PageIndex, model.PageSize)
			);
		}

		public ActionResult Search()
		{
			string sSearchValue = RouteData.Values["id"].ToString();
			var model = new BlogPaginationModel(db, BlogFilterType.Search, sSearchValue);
			model.AJAXUrl = "~/Blog/SearchPaginate";
			return View("Index", model);
		}

		public ActionResult SearchPaginate(PaginationModel model)
		{
			// deserialize the data
			var additionalData = JsonConvert.DeserializeObject<Dictionary<string, string>>(model.AdditionalData);

			// cast the value to the event type we want
			BlogFilterType filterType = (BlogFilterType)int.Parse(additionalData["FilterType"]);
			string filterValue = additionalData["FilterValue"];

			return PartialView(
				"~/Views/Blog/BlogPostsPaginationBody.cshtml",
				new BlogPaginationModel(db, filterType, filterValue, model.PageIndex, model.PageSize)
			);
		}

		public ViewResult Adapt(HttpPostedFileBase blogXmlFile)
		{
			IEnumerable<BlogPost> blogs = new List<BlogPost>();

			if (blogXmlFile == null || blogXmlFile.ContentLength == 0)
			{
				//return Request.CreateResponse(HttpStatusCode.BadRequest, new BlogPost());
			}
			else
			{
				XDocument blogsLinq = XDocument.Load(blogXmlFile.InputStream);

				var authorsRef = blogsLinq.Descendants("authors").Select(x => new
				{
					Id = x.Attribute("id").Value,
					Email = x.Attribute("email").Value,
					Title = x.Element("title").Value //Source XML uses CDATA node here
				}).ToDictionary(a => a.Id, a => a);


				var categoriesRef = blogsLinq.Descendants("categories");

				var posts = blogsLinq.Descendants("posts").Elements("post");

				blogs = posts.Select(x =>
					new BlogPost
					{
						Title = x.Element("Title").Value,
						Content = x.Element("Content").Value
					}
				);

			}

			return View( "~/Views/Blog/Adapt.cshtml", blogs);
			//return Request.CreateResponse(HttpStatusCode.OK, blogs);
		} 

    }
}
