using System.ComponentModel.DataAnnotations;

namespace Labb2Bibliotek.Models
{
	public class Borrower
	{
		[Key]
		public int LoanCard { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
	}
}
