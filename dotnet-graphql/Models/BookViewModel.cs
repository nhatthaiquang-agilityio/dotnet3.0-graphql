using System.Collections.Generic;

namespace dotnet_graphql.Models
{
    public class BookViewModel
    {
        public int Id;
        public string BookName { get; set; }
        public decimal Price { get; set; }
        public List<string> BookCategories { get; set; }

        public int AuthorId { get; set; }
        public string Author { get; set; }
    }
}
