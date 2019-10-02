using System.ComponentModel.DataAnnotations;

namespace dotnet_graphql.Models
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }
    }
}