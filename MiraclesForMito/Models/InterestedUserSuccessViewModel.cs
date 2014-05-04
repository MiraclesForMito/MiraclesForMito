using System.Net.Mail;

namespace MiraclesForMito.Models
{
	/// <summary>
	/// Passed to InterestedSuccess View
	/// Contains the Model; all information submitted by the user
	/// And the would-be email message sent to Miracles for Mito staff
	/// </summary>
	public class InterestedSuccessViewModel
	{
		public InterestedUserModel InterestedUserModel { get; set; }
		public MailMessage EmailMessage { get; set; }
		public string SendTo { get; set; }
		public bool SuccessfullySentEmail { get; set; }

		/// <summary>
		/// Safe to use inside href attribute as body: property
		/// </summary>
		public string SimpleEmailMessage
		{
			get
			{
				if (InterestedUserModel!=null){
					return string.Format(@"
First - {0} 
Last - {1} 
Email - {2} 
Phone - {3} 
Interest - {4}", 
						InterestedUserModel.FirstName ?? "No first name",
						InterestedUserModel.LastName ?? "No last name",
						InterestedUserModel.Email ?? "No email",
						InterestedUserModel.PhoneNumber ?? "No phone number",
						InterestedUserModel.InterestedIn
					);
				}

				return "No info provided";

			}
		}

	}

}