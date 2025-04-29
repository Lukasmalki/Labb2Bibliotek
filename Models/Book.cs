using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Labb2Bibliotek.Models
{
	public class Book
	{
		public int BookId { get; set; }
		public string Title { get; set; } = null!;
		public long Isbn { get; set; }
		public int PublicationYear { get; set; }
		public float Rating { get; set; }
		//public Author Author { get; set; } = null!;
		//public bool IsBorrowed { get; set; }
		public int CopiesTotal { get; set; }
		public ICollection<Author> Authors { get; set; } = null!;

	}
}
