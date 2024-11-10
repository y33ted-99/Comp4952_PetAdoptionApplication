using System.Collections.Generic;
using PetAdoptionApp.Models;

namespace PetAdoptionApp.ViewModels
{
    public class PetSearchViewModel
    {
        // Search Criteria
        public string Name { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        // Search Results
        public List<Pet> Pets { get; set; }
    }
}
