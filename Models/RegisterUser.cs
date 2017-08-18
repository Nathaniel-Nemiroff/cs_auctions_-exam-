using System.ComponentModel.DataAnnotations;

namespace proj
{
	public class RegisterUser : basemodel
	{
		[Required]
		[RegularExpression(@"^[a-zA-Z]+$")]
		[Display(Name="First Name")]
		public string First{get;set;}
		[Required]
		[RegularExpression(@"^[a-zA-Z]+$")]
		[Display(Name="Last Name")]
		public string Last{get;set;}

		[Required]
		[MinLength(3)]
		[MaxLength(20)]
		public string Username{get;set;}

		[Required]
		[MinLength(8)]
		[DataType(DataType.Password)]
		public string Password{get;set;}

		[Compare("Password", ErrorMessage = "Password and confirmation must match.")]
		[DataType(DataType.Password)]
		[Display(Name="Confirm Password")]
		public string Confirm{get;set;}
	}
}
