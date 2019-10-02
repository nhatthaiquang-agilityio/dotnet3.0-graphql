using System.Collections.Generic;

namespace dotnet_graphql.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }

        public int ProductTypeId { get; set; }

        public ProductType ProductType { get; set; }

        public string Description { get; set; }

        public int ProductBrandId { get; set; }

        public ProductBrand ProductBrand { get; set; }

        // Quantity in stock
        public int AvailableStock { get; set; }

        public ICollection<string> Sizes { get; set; }
    }
}