using System.ComponentModel.DataAnnotations;

namespace PetAdoptionApp.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Species { get; set; }

        public int Age { get; set; }

        public string Breed { get; set; }

        public string ImageUrl { get; set; }

        // Property to mark pets as favorites
        public bool IsFavorite { get; set; }

        // New Description property
        public string Description { get; set; }
    }
}
