using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HomeWork_5.Models;
using HomeWork_5.ViewModels;

namespace HomeWork_5.Controllers
{
    public class HomeController : Controller
    {
        // Thread lock object to protect in-memory collections across requests
        private static readonly object _lock = new object();

        // Static in-memory data store
        private static readonly List<Author> Authors = new List<Author>
        {
            new Author { Id = 1, Name = "ME", Country = "UK" },
            new Author { Id = 2, Name = "You", Country = "USA" }
        };

        private static readonly List<Book> Books = new List<Book>
        {
            new Book { Id = 1, Title = "LORD OF RINGS", Price = 19.99m, AuthorId = 1 },
            new Book { Id = 2, Title = "HOUSE OF BLOOD", Price = 24.99m, AuthorId = 2 },
            new Book { Id = 3, Title = "GUARDIANS OF GALAXY", Price = 22.50m, AuthorId = 2 }
        };

        public IActionResult Index()
        {
            List<BookCardViewModel> bookCardList;
            List<Author> authorList;

            lock (_lock)
            {
                bookCardList = Books.Select(b => new BookCardViewModel
                {
                    Id = b.Id,
                    Title = b.Title ?? string.Empty,
                    Price = b.Price,
                    AuthorName = Authors.FirstOrDefault(a => a.Id == b.AuthorId)?.Name ?? "Unknown"
                }).ToList();

                authorList = Authors.ToList(); // Return copy for thread safety
            }

            var viewModel = new HomeViewModel
            {
                Books = bookCardList,
                Authors = authorList
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddBook(string title, decimal price, int authorId)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                lock (_lock)
                {
                    int newId = Books.Any() ? Books.Max(b => b.Id) + 1 : 1;
                    Books.Add(new Book
                    {
                        Id = newId,
                        Title = title,
                        Price = price,
                        AuthorId = authorId
                    });
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddAuthor(string name, string country)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                lock (_lock)
                {
                    int newId = Authors.Any() ? Authors.Max(a => a.Id) + 1 : 1;
                    Authors.Add(new Author
                    {
                        Id = newId,
                        Name = name,
                        Country = country ?? string.Empty
                    });
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveBook(int id)
        {
            lock (_lock)
            {
                var book = Books.FirstOrDefault(b => b.Id == id);
                if (book != null)
                {
                    Books.Remove(book);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}