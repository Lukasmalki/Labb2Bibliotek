using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Labb2Bibliotek.Models;
using Labb2Bibliotek.DTOs;

namespace Labb2Bibliotek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var authors = await _context.Books
                .Include(b => b.Authors)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Isbn = b.Isbn,
                    PublicationYear = b.PublicationYear,
                    Rating = b.Rating,
                    CopiesTotal = b.CopiesTotal,
                    Authors = b.Authors.Select(a => new Author
                    {
                        AuthorId = a.AuthorId,
                        FirstName = a.FirstName,
                        LastName = a.LastName
                    }).ToList()  
                }).ToListAsync();

            return authors;
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(BookDTO bookDTO)
        {

			var authorsFromDb = await _context.Authors
								  .Where(a => bookDTO.AuthorIds.Contains(a.AuthorId))
								  .ToListAsync();

            var isbnFromDb = await _context.Books.Where(b => b.Isbn == bookDTO.Isbn).ToListAsync();

			if (isbnFromDb.Any())
			{
				return BadRequest("ISBN already exists.");
			}

			if (authorsFromDb.Count != bookDTO.AuthorIds.Count)
			{
				return NotFound("Some authors were not found.");
			}

			var book = new Book()
			{
				Title = bookDTO.Title,
                Isbn = bookDTO.Isbn,
                PublicationYear = bookDTO.PublicationYear,
                Rating = bookDTO.Rating,
                CopiesTotal = bookDTO.CopiesTotal,
                Authors = authorsFromDb
			};

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.BookId }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
