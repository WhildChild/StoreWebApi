namespace StoreWebApi.Models
{
    public class ProductDb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public int Count { get; set; }
    }
}
