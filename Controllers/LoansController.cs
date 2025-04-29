using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Labb2Bibliotek.Models;
using Labb2Bibliotek.DTOs;
using System.Net;

namespace Labb2Bibliotek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
        {
			return await _context.Loans.Include(l => l.Book).Include(l => l.Borrower).ToListAsync();
		}

		// GET: api/Loans/5
		[HttpGet("{id}")]
        public async Task<ActionResult<Loan>> GetLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            return loan;
        }

        // PUT: api/Loans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoan(int id)
        {
            var bookLoan = await _context.Loans.Include(l => l.Book)
                    .FirstOrDefaultAsync(l => l.LoanId == id);

            if (bookLoan == null)
            {
                return NotFound("Loan not found");
            }
            else
            {
                bookLoan.ReturnDate = DateTime.Now;
            }

			//var loan = new Loan()
			//{

			//};





			_context.Entry(bookLoan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
                {
                    return NotFound();
                }
                else
                {
					throw;
				}
            }

            return Ok($"Return complete and made at {bookLoan.ReturnDate}");
        }

        // POST: api/Loans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Loan>> PostLoan(LoanDTO loanDTO)
        {
            var bookId = loanDTO.BookId;
            Book? bookFromDb = _context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (bookFromDb == null)
            {
                return NotFound();
            }

            //else
            //{

            //    bookFromDb.IsBorrowed = true;
            //}

            var copiesTotalThatExist = bookFromDb.CopiesTotal;


            var numberOfNotAvailableCopies = _context.Loans.Where(l => l.Book.BookId == bookId && l.BorrowDate != null && l.ReturnDate == null).Count();

            var copiesAvailable = copiesTotalThatExist - numberOfNotAvailableCopies;

            if(copiesAvailable <= 0)
            {
                return BadRequest("That book is not available");
            }



            var borrowerId = loanDTO.BorrowerId;
            Borrower? borrowerFromDb = _context.Borrowers.FirstOrDefault(b => b.LoanCard == borrowerId);
            if (borrowerFromDb == null)
            {
                return NotFound();
            }




            var loan = new Loan
            {
                Borrower = borrowerFromDb,
                Book = bookFromDb,
                BorrowDate = DateTime.Now,
                ReturnDate = null
                
            };
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoan", new { id = loan.LoanId }, loan);
        }

        // DELETE: api/Loans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.LoanId == id);
        }
    }
}
