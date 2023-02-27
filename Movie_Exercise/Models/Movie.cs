using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Movie_Exercise.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Director { get; set; }
        [Required]
        public int ReleaseYear { get; set; }
        [Required]
        public int Price { get; set; }

    }
}
