using Microsoft.EntityFrameworkCore;
using System.Numerics;
using Labb2Bibliotek.Models;

namespace Labb2Bibliotek.Models
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Book> Books { get; set; }
		public DbSet<Borrower> Borrowers { get; set; }
		public DbSet<Loan> Loans { get; set; }
		public DbSet<Author> Authors { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		//public DbSet<Labb2Bibliotek.Models.Author> Author { get; set; } = default!;
	}
}
