using AssignmentTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Diagnostics;
using AssignmentDataLinkLayer.ViewModels;
using AssignmentDataLinkLayer.DataContext;
using AssignmentDataLinkLayer.DataModels;

namespace AssignmentTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public enum GenreTypes
        {
            Thriller = 1,
            Romance = 2,
            SciFi = 3,
            Fantasy = 4
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult BookRecords()
        {
            List<Book> model = _context.Books.ToList();
            return View(model);
        }
        public IActionResult AddBookDetails(string bookName, string author, string borrowerName, DateTime dateOfIssue, int genre, string city)
        {
            Borrower borrower = new Borrower();
            borrower.City = city;
            borrower.Name = borrowerName;
            _context.Borrowers.Add(borrower);
            _context.SaveChanges();

            Book books = new Book();
            books.BookName = bookName;
            books.Author = author;
            books.DateOfIssue = dateOfIssue;
            books.Genre = genre;
            books.City = city;
            books.BorrowerName = borrowerName;
            books.BorrowerId = borrower.Id;

            _context.Books.Add(books);
            _context.SaveChanges();

            return RedirectToAction("BookRecords");
        }
        public IActionResult EditBookDetails(int bookId, int borrowerId, string bookName, string author, string borrowerName, DateTime dateOfIssue, int genre, string city)
        {


            Borrower borrower = _context.Borrowers.FirstOrDefault(borrower => borrower.Id == borrowerId);
            if (borrower == null)
            {
                Borrower newBorrower = new Borrower();
                newBorrower.City = city;
                newBorrower.Name = borrowerName;
                _context.Borrowers.Add(newBorrower);
            }
            else
            {
                borrower.City = city;
                borrower.Name = borrowerName;
                _context.Borrowers.Add(borrower);
            }
            _context.SaveChanges();

            Book books = _context.Books.FirstOrDefault(book => book.Id == bookId);
            if (books != null)
            {
                books.BookName = bookName;
                books.Author = author;
                books.DateOfIssue = dateOfIssue;
                books.Genre = genre;
                books.City = city;
                books.BorrowerName = borrowerName;
                books.BorrowerId = borrower.Id;
            }

            _context.Books.Add(books);
            _context.SaveChanges();

            return RedirectToAction("BookRecords");
        }



        public IActionResult DeleteBookRecord(int bookId)
        {
            Book deleteBook = _context.Books.FirstOrDefault(book => book.Id == bookId);
            if (deleteBook != null)
            {
                _context.Books.Remove(deleteBook);
                _context.SaveChanges();
            }
            return RedirectToAction("BookRecords");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
