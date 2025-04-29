namespace Labb2Bibliotek.Models
{
	public class Author
	{
		public int AuthorId { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public ICollection<Book> Books { get; set; } = null!;
	}
}
