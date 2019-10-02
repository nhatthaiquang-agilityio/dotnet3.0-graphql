using System.Collections.Generic;

namespace dotnet_graphql.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; }
    }
}