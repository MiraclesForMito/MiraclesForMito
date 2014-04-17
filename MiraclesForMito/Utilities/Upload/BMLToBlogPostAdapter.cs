using MiraclesForMito.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace MiraclesForMito.Controllers
{
	interface IBlogPostsAdapter
	{
		IEnumerable<BlogPost> BlogPosts { get; }

	}

	/// <summary>
	/// BML is BlogML, which is the language used from old MiraclesForMito system
	/// </summary>
	class BMLToBlogPostAdapter : IBlogPostsAdapter
	{
		private string BMLNamespace = "http://www.blogml.com/2006/09/BlogML";

		Stream XmlStream;
		
		/// <summary>
		/// Parsed to convert to IEnumerable<BlogPosts></BlogPosts>
		/// </summary>
		XDocument XmlDocument;

		Dictionary<string, Author> AuthorLookup {
			get {
						 return this.XmlDocument
										.Descendants(Ns("authors"))
										.SelectMany(authors => authors.Elements(Ns("author"))
											.Select(author => new Author {
												Id = author.Attribute("id").Value,
												DateCreated = author.Attribute(Ns("date-created")).Value,
												DateModified = author.Attribute(Ns("date-modified")).Value,
												Email = author.Attribute(Ns("email")).Value,
												Title = author.Value,
												
											}))
										.GroupBy(author => author.Id)
										.ToDictionary(authorGroup => authorGroup.Key, authorGroup => authorGroup.FirstOrDefault());
			}

		}


		public BMLToBlogPostAdapter(Stream xmlStream)
		{
			this.XmlStream = xmlStream;

			//XmlReader reader = XmlReader.Create(xmlStream);
			this.XmlDocument = XDocument.Load(xmlStream);

		}

		private string Ns(string s)
		{
			return String.Format("{{{0}}}{1}", BMLNamespace, s);
		}
		public IEnumerable<BlogPost>  BlogPosts
		{ 
			get {

				if (!IsValid())
				{
					return new List<BlogPost>();
				}
				else
				{

					return this.XmlDocument
								.Descendants(Ns("posts"))
								.SelectMany(posts => posts.Elements(Ns("post"))
									.Select(BlogPostFromBMLPost)
								);
				}

			}
		}

		private BlogPost BlogPostFromBMLPost(XElement bmlPost)
		{
			BlogPost blogPost = new BlogPost();

			//ID = post.Attribute("id").Value,

			blogPost.Title		= bmlPost.Element(Ns("title")).Value;
			blogPost.Author		= bmlPost.Elements(Ns("authors")) 
									.SelectMany(authors=> authors.Elements(Ns("author"))
										.Select(author => author.Attribute("ref").Value ))	//No need to use the Lookup?
									.FirstOrDefault();										//Instead of string-joining with a comma-separator? 
			blogPost.SEOLink		= SetSEOLinkValFromTitle(bmlPost.Element(Ns("title")).Value);
			blogPost.Content		= bmlPost.Element(Ns("content")).Value;
			blogPost.ContentRaw	= bmlPost.Element(Ns("content")).Value;
			blogPost.InsertDate	= DateTime.Parse(bmlPost.Attribute("date-created").Value);
			blogPost.UpdatedDate = DateTime.Parse(bmlPost.Attribute("date-modified").Value);
			blogPost.Published	= bmlPost.Attribute("is-published").Value.Equals("true", StringComparison.CurrentCultureIgnoreCase);

			return blogPost; 
										
										
		}

		private string  SetSEOLinkValFromTitle(string sVal){
				
			// we only want letters, numbers, hyphens and spaces
			var sanitizePattern = "[^A-Za-z0-9 \\-]";

			var sanitizeRE = new Regex(sanitizePattern, RegexOptions.IgnoreCase);

			// sanitize the value
			var sSanitized = Regex.Replace(sVal, sanitizePattern, "");

			// we want the urls to have dashes instead of spaces to make them more SEO Friendly
			var sSEOFriendly = Regex.Replace(sSanitized, " ", "-");

			// we don't want the url to be longer than 60 characters
			sSEOFriendly = sSEOFriendly.Substring(0, Math.Min(60, sSEOFriendly.Length)).TrimEnd('-');

			// replace multiple hyphen instances
			sSEOFriendly = Regex.Replace(sSEOFriendly, "-{2,}", "-");

			return sSEOFriendly;
		}

		private bool IsValid()
		{
			bool valid = false;
			try
			{

				valid = this.XmlDocument != null;
				//valid &= this.XmlDocument.Descendants("authors").Count() == 1;
				//valid &= this.XmlDocument.Descendants("posts").Count() > 1;


			} catch(Exception) {


			} 

			return valid;
		}
	}
}
