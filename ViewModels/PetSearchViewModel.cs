using System.Collections.Generic;
using PetAdoptionApp.Models;

namespace PetAdoptionApp.ViewModels
{
    public class PetSearchViewModel
    {
        public string Name { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        // New property for tags
        public string Tags { get; set; }

        public List<Pet> Pets { get; set; }
    }
}
