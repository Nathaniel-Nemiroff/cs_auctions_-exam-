using System;
using System.ComponentModel.DataAnnotations;

namespace proj
{
	public class User : basemodel
	{
		public string Username{get;set;}
		public string First{get;set;}
		public string Last{get;set;}
		public string Password{get;set;}
		public int Balance {get;set;}

		public DateTime CreatedAt{get;set;}
		public DateTime UpdatedAt{get;set;}

		public User()
		{
			Balance = 1000;
			CreatedAt = DateTime.Now;
			UpdatedAt = DateTime.Now;
		}
	}
}
