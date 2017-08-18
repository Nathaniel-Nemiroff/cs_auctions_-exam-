using System;
using System.ComponentModel.DataAnnotations;

namespace proj
{
	public class PostAuction : basemodel
	{
		[Required]
		[MinLength(3)]
		[Display(Name="Product Name")]
		public string Name{get;set;}

		[Required]
		[MinLength(10)]
		public string Description{get;set;}

		[Range(1, int.MaxValue, ErrorMessage = "Bid must be greater than 0")]
		[Display(Name="Starting Bid")]
		public int Bid{get;set;}

		[Required]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? EndsAt{get;set;}
		
		public PostAuction()
		{
			Bid = 0;
			EndsAt = DateTime.Now;
		}
	}
}
