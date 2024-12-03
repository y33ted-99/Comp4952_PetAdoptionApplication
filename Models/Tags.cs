using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetAdoptionApp.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tag name is required.")]
        public string Name { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<Pet> Pets { get; set; }
    }
}
