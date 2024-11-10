using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAdoptionApp.Data;
using PetAdoptionApp.Models;
using PetAdoptionApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace PetAdoptionApp.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PetsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

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

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Pet pet, IFormFile ImageUrl)
        {
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ImageUrl.CopyToAsync(memoryStream);
                    pet.ImageUrl = memoryStream.ToArray();
                }
            }
            else
            {
                ModelState.AddModelError("ImageUrl", "An image is required.");
            }

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
            var pets = await _context.Pets.Where(p => p.IsFavorite).ToListAsync();
            return View(pets);
        }

        // Toggle favorite status
        [HttpPost]
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
