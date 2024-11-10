using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAdoptionApp.Data;
using PetAdoptionApp.Models;
using Microsoft.EntityFrameworkCore;
using PetAdoptionApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;

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

            return RedirectToAction("Index", "Home");
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

        public async Task<IActionResult> Search(PetSearchViewModel searchModel)
        {
            var query = _context.Pets.AsQueryable();

            // Apply filters based on search criteria
            if (!string.IsNullOrEmpty(searchModel.Name))
            {
                query = query.Where(p => p.Name.Contains(searchModel.Name));
            }

            if (!string.IsNullOrEmpty(searchModel.Species))
            {
                query = query.Where(p => p.Species == searchModel.Species);
            }

            if (!string.IsNullOrEmpty(searchModel.Breed))
            {
                query = query.Where(p => p.Breed == searchModel.Breed);
            }

            if (searchModel.MinAge.HasValue)
            {
                query = query.Where(p => p.Age >= searchModel.MinAge.Value);
            }

            if (searchModel.MaxAge.HasValue)
            {
                query = query.Where(p => p.Age <= searchModel.MaxAge.Value);
            }

            // Execute the query and get the list of pets
            searchModel.Pets = await query.ToListAsync();

            return View(searchModel);
        }


            

    }  
}
