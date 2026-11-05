using System.Collections.Generic;
using HomeWork_5.Models;

namespace HomeWork_5.ViewModels
{
    public class HomeViewModel
    {
        public List<BookCardViewModel> Books { get; set; }
        public List<Author> Authors { get; set; }
    }
}