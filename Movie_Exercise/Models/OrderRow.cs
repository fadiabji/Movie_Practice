using System.ComponentModel.DataAnnotations;

namespace Movie_Exercise.Models
{
    public class OrderRow
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int MovieId { get; set; }
        [Required]
        public int Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
