namespace Labb2Bibliotek.Models
{
	public class Loan
	{
		public int LoanId { get; set; }
		public Book Book { get; set; } = null!;
		public Borrower Borrower { get; set; } = null!;
		public DateTime BorrowDate { get; set; }
		public DateTime? ReturnDate { get; set; }
	}
}
