using System.ComponentModel.DataAnnotations;

namespace Project_CRUDwithoutEF_.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author{ get; set; }

        [Range(1,int.MaxValue,ErrorMessage ="Value should be equal or greater than 1")]
        public int Price { get; set; }
    }
}
