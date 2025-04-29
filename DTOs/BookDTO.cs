using Labb2Bibliotek.Models;

namespace Labb2Bibliotek.DTOs
{
	public class BookDTO
	{
		public string Title { get; set; } = null!;
		public long Isbn { get; set; }
		public int PublicationYear { get; set; }
		public float Rating { get; set; }
		public int CopiesTotal { get; set; }
		//public int AuthorId { get; set; }
		public List<int> AuthorIds { get; set; } = null!;

	}
}
