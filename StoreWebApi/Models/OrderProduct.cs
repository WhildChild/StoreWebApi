using System.ComponentModel.DataAnnotations;

namespace StoreWebApi.Models
{
    public class OrderProduct
    {
        [Key]
        public int Id { get; set; }
        public int PositionId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public double ProductPrice { get; set; }
        public int ProductCount { get; set; }
    }
}
