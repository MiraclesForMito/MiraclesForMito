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

	class XmlToBlogPostsAdapter : IBlogPostsAdapter
	{
		Stream XmlStream;
		
		/// <summary>
		/// Parsed to convert to IEnumerable<BlogPosts></BlogPosts>
		/// </summary>
		XDocument XmlDocument;

		Dictionary<string, Author> AuthorLookup {
			get {
						 return this.XmlDocument
										.Descendants("authors")
										.SelectMany(authors => authors.Elements("author")
											.Select(author => new Author {
												Id = author.Attribute("id").Value,
												DateCreated = author.Attribute("date-created").Value,
												DateModified = author.Attribute("date-modified").Value,
												Email = author.Attribute("email").Value,
												Title = author.Value,
												
											}))
										.GroupBy(author => author.Id)
										.ToDictionary(authorGroup => authorGroup.Key, authorGroup => authorGroup.FirstOrDefault());
			}

		}

		public XmlToBlogPostsAdapter(Stream xmlStream)
		{
			this.XmlStream = xmlStream;

			//XmlReader reader = XmlReader.Create(xmlStream);
			this.XmlDocument = XDocument.Load(xmlStream);

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
								.Descendants("posts")
								.SelectMany(posts => posts.Elements("post")
									.Select(post => new BlogPost {
										//ID = post.Attribute("id").Value,
										Title		= post.Element("title").Value,
										Author		= post.Elements("authors") 
														.SelectMany(authors=> authors.Elements("author")
															.Select(author => author.Attribute("ref").Value ))	//No need to use the Lookup?
														.FirstOrDefault(),										//Instead of string-joining with a comma-separator? 
										SEOLink		= SetSEOLinkValFromTitle(post.Element("title").Value),
										Content		= post.Element("content").Value,
										ContentRaw	= post.Element("content").Value,
										InsertDate	= DateTime.Parse(post.Attribute("date-created").Value),
										UpdatedDate = DateTime.Parse(post.Attribute("date-modified").Value),
										Published	= post.Attribute("is-publised").Value.Equals("true", StringComparison.CurrentCultureIgnoreCase) }
									)
								);
				}

			}
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
			sSEOFriendly = sSEOFriendly.Substring(0, 60).TrimEnd('-');

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
