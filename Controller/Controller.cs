using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace proj
{
	public class projController:Controller
	{
		private projContext _context;

		public projController(projContext context)
		{
			_context = context;
		}

		[HttpGet]
		[Route("")]
		public IActionResult Index()
		{
			int userid = -1;
			if(HttpContext.Session.GetInt32("user") != null)
				userid = (int)HttpContext.Session.GetInt32("user");

			if(userid >= 0)
			{
				if(HttpContext.Session.GetString("ClaimErr")!= null)
				{
					ViewBag.ClaimErr = (string)HttpContext.Session.GetString("ClaimErr");
					HttpContext.Session.Remove("ClaimErr");
				}
				ViewBag.User = _context.Users.SingleOrDefault(u=>u.id==userid);
				ViewBag.Auctions = _context.Auctions
									.Include(a=>a.Seller)
									.OrderBy(a=>a.EndsAt);
				return View("IndexLoggedIn");
			}
			ViewBag.Log = HttpContext.Session.GetString("Log");
										HttpContext.Session.SetString("Log", "");
			ViewBag.errors = new List<string>();
			return View();
		}
		[HttpPost]
		[Route("login")]
		public IActionResult Login(string name, string pass)
		{
			User log = _context.Users.SingleOrDefault(u=>u.Username==name);
			HttpContext.Session.SetString("Log", "Login Failed");
			if(log != null)
				if(log.Password == pass)
				{
					HttpContext.Session.SetInt32("user", log.id);
					HttpContext.Session.SetString("Log", "");
				}
			return RedirectToAction("Index");
		}
		[HttpGet]
		[Route("logout")]
		public IActionResult Logout()
		{
			HttpContext.Session.SetInt32("user", -1);
			return RedirectToAction("Index");
		}
		[HttpPost]
		[Route("")]
		public IActionResult Register(RegisterUser model)
		{

			/*

					I do not remember if there is a way to preserv
					Modelstate.Values between controller actions,
					so I resolved to have two views point toward
					the path "/"

			*/

			if(ModelState.IsValid)
			{
				User NewUser = new User
				{
					Username = model.Username,
					First = model.First,
					Last = model.Last,
					Password = model.Password,
				};
				_context.Add(NewUser);
				_context.SaveChanges();
			}
			else
			ViewBag.errors = ModelState.Values;
			return View("Index");
		}


		[HttpGet][Route("newauction")]
		public IActionResult NewAuction()
		{
			if(HttpContext.Session.GetInt32("user") != null)
				if(HttpContext.Session.GetInt32("user") >= 0)
					return View();
			return RedirectToAction("Index");
		}

		[HttpPost][Route("newauction")]
		public IActionResult PostAuction(PostAuction model)
		{
			if(ModelState.IsValid)
			{
				if(model.EndsAt < DateTime.Now)
				{
					ViewBag.EndsErr = "Date must be in the future";
					return View("NewAuction");	
				}
				Auction NewAuction = new Auction
				{
					Name = model.Name,
					Description = model.Description,
					Bid = model.Bid,
					EndsAt = (DateTime)model.EndsAt,
					SellerId = (int)HttpContext.Session.GetInt32("user"),
					BuyerId = (int)HttpContext.Session.GetInt32("user"),
				};
				_context.Add(NewAuction);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.errors = ModelState.Values;
			return View("NewAuction");
		}
		[HttpGet][Route("delete/{id}")]
		public IActionResult Delete(int id)
		{
			int? userid = HttpContext.Session.GetInt32("user");
			if(userid != null)
				if(userid >= 0)
				{
					Auction prune = _context.Auctions.SingleOrDefault(a=>a.id==id);
					if(userid != prune.SellerId)
						return RedirectToAction("Index");
					_context.Auctions.Remove(prune);
					_context.SaveChanges();
				}
			return RedirectToAction("Index");	
		}

		[HttpGet][Route("auction/{id}")]
		public IActionResult Auction(int id)
		{
			ViewBag.User    = _context.Users
								.SingleOrDefault(u=>u.id==
								HttpContext.Session.GetInt32("user"));
			ViewBag.Auction = _context.Auctions
						.Include(a=>a.Seller).Include(a=>a.Buyer)
						.SingleOrDefault(a=>a.id==id);
			return View();
		}
		[HttpPost][Route("newbid/{id}")]
		public IActionResult NewBid(int bid, int id)
		{
			if(HttpContext.Session.GetInt32("user") != null)
				if(HttpContext.Session.GetInt32("user") >= 0)
				{
					User bidder = _context.Users
								.SingleOrDefault(u=>u.id==
								HttpContext.Session.GetInt32("user"));
					Auction item = _context.Auctions
								.Include(a=>a.Seller).Include(a=>a.Buyer)
								.SingleOrDefault(a=>a.id==id);

					if(bidder.Balance < bid)
					{
						ViewBag.BidErr = "You don't have that kind of money!";
						ViewBag.Auction = item;
						ViewBag.User = bidder;
						return View("Auction");
					}
					if(bid < item.Bid)
					{
						ViewBag.BidErr = "Current bid is higher than " + bid;
						ViewBag.Auction = item;
						ViewBag.User = bidder;
						return View("Auction");
					}
					item.Bid = bid;
					item.BuyerId = bidder.id;
					_context.SaveChanges();
				}
			return RedirectToAction("Index");	
		}

		[HttpGet][Route("claim/{id}")]
		public IActionResult Claim(int id)
		{


			/*

					There are cases when a user will be eligable for claiming 
					an item, the time limit is up and they is the highest bidder,
					but then they will then not be able to afford the product.
					In this case, I have chosen to resolve this by leaving 
					the auction listing up until the user can afford it again,
					instead of making that user go into a negative Balance.

			*/

			int? userid = HttpContext.Session.GetInt32("user");
			if(userid != null)
				if(userid >= 0)
				{
					Auction prune = _context.Auctions.SingleOrDefault(a=>a.id==id);
					if(userid == prune.BuyerId)
					{
						User buyer = _context.Users.SingleOrDefault(u=>u.id == prune.BuyerId);
						User seller = _context.Users.SingleOrDefault(u=>u.id == prune.SellerId);
						if(buyer.Balance < prune.Bid)
						{
							HttpContext.Session.SetString("ClaimErr","You can't afford that anymore!");
						}
						else
						{
							buyer.Balance -= prune.Bid;
							seller.Balance += prune.Bid;
							_context.Auctions.Remove(prune);
							_context.SaveChanges();
						}
					}
				}
			return RedirectToAction("Index");	
		}

		[HttpGet][Route("renew/{id}")]
		public IActionResult Renew(int id)
		{
			int? userid = HttpContext.Session.GetInt32("user");
			if(userid != null)
				if(userid >= 0)
				{
					Auction renew = _context.Auctions.SingleOrDefault(a=>a.id==id);
					if(userid != renew.SellerId)
						return RedirectToAction("Index");
					renew.EndsAt = DateTime.Now.AddHours(24);
					_context.SaveChanges();
				}
			return RedirectToAction("Index");	
		}

		[HttpPost][Route("fastforward")]
		public IActionResult FastForward()
		{

			/*

					This function is purely for the demonstration of the
					item claiming functionality, otherwise it would take
					hours or even days to see if the app works correctly

			*/

			List<Auction> Listings = _context.Auctions.ToList();
			foreach(var i in Listings)
			{
				if(i.EndsAt > DateTime.Now)
					i.EndsAt = i.EndsAt.AddHours(-1);
			}
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

	}
}
