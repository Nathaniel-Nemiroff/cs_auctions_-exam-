using System;
using System.ComponentModel.DataAnnotations;

namespace proj
{
	public class Auction : basemodel
	{
		public string Name{get;set;}
		public string Description{get;set;}
		public int Bid{get;set;}
		public DateTime EndsAt{get;set;}

		public int SellerId{get;set;}
		public User Seller{get;set;}

		public int BuyerId{get;set;}
		public User Buyer{get;set;}

		public DateTime CreatedAt{get;set;}
		public DateTime UpdatedAt{get;set;}

		public Auction()
		{
			Bid = 0;
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
		}
	}
}
