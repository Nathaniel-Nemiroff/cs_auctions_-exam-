using Microsoft.EntityFrameworkCore;

namespace proj
{
	public class projContext : DbContext
	{
		public projContext(DbContextOptions<projContext> options):base(options){}
		public DbSet<User> Users {get;set;}
		public DbSet<Auction> Auctions {get;set;}
	}
}
