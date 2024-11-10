using System.ComponentModel.DataAnnotations;

namespace PetAdoptionApp.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Species is required.")]
        public string Species { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Breed is required.")]
        public string Breed { get; set; }

        public byte[] ImageUrl { get; set; }

        public bool IsFavorite { get; set; }

        public string Description { get; set; }
    }
}
