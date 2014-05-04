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

	}

}