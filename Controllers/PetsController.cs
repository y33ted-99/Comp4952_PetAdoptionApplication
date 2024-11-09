using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAdoptionApp.Data;
using PetAdoptionApp.Models;

namespace PetAdoptionApp.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display list of pets on the homepage
        public async Task<IActionResult> Index()
        {
            var pets = await _context.Pets.ToListAsync();
            return View(pets);
        }

        // View details of a specific pet
        public async Task<IActionResult> Details(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            return View(pet);
        }

        // Add a new pet
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Pet pet)
        {
            if (ModelState.IsValid)
            {
                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pet);
        }

        // Display favorite pets
        public async Task<IActionResult> Favorites()
        {
            // Logic to retrieve favorite pets
            var pets = await _context.Pets.Where(p => p.IsFavorite).ToListAsync();
            return View(pets);
        }

        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }

            pet.IsFavorite = !pet.IsFavorite;  // Toggle the favorite status
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Filter(string species, int? minAge, int? maxAge, string breed)
        {
            var pets = _context.Pets.AsQueryable();

            if (!string.IsNullOrEmpty(species))
            {
                pets = pets.Where(p => p.Species == species);
            }

            if (minAge.HasValue)
            {
                pets = pets.Where(p => p.Age >= minAge);
            }

            if (maxAge.HasValue)
            {
                pets = pets.Where(p => p.Age <= maxAge);
            }

            if (!string.IsNullOrEmpty(breed))
            {
                pets = pets.Where(p => p.Breed.Contains(breed));
            }

            return View(await pets.ToListAsync());
        }

    }  
}
