using System.ComponentModel.DataAnnotations;

namespace StoreWebApi.Models
{
    public class CustomerDb
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string TelephoneNumber { get; set; }
    }
}
