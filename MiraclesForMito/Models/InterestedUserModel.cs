using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiraclesForMito.Models
{
	public class InterestedUserModel
	{
		/// <summary>
		/// Interested person'sFirst Name
		/// </summary>
		[Display(Name="First Name", GroupName="Name", ShortName="First", Description="First Name", Order=1)]
		[Required]
		public string FirstName { get; set; }

		/// <summary>
		/// Interested person's Last Name
		/// </summary>
		[Display(Name = "Last Name", GroupName = "Name", ShortName = "Last", Description = "Last Name", Order = 2)]
		public string LastName { get; set; }

		/// <summary>
		/// Is required and must be a valid email.
		/// </summary>
		[Required]
		[Display(Name="Email Address", GroupName="Contact", ShortName="Email", Order=3)]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }


		/// <summary>
		/// Is required and must be a valid phone
		/// </summary>
		[Display(Name = "Phone Number", GroupName = "Contact", ShortName="Phone", Description="Phone", Order=4)]
		[DataType(DataType.PhoneNumber)]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// What person wants specifically
		/// </summary>
		[Required]
		[UIHint("DropdownCamelCase")]
		public InterestedIn InterestedIn { get; set; }

	}

	public enum InterestedIn
	{
		SupportForMyChild,
		SupportForMe,
		Volunteering,
		Donations
	}
}