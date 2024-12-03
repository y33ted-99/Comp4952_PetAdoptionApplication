using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAdoptionApp.Data;
using PetAdoptionApp.Models;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PetAdoptionApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Display list of pets on the homepage
        public async Task<IActionResult> Index()
        {
            var pets = await _context.Pets.Include(p => p.Tags).ToListAsync();
            return View(pets);
        }

        // View details of a specific pet
        public async Task<IActionResult> Details(int id)
        {
            var pet = await _context.Pets.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null)
            {
                return NotFound();
            }
            return View(pet);
        }

        // Add a new pet (GET)
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // Add a new pet (POST)
        [HttpPost]
        public async Task<IActionResult> Add(Pet pet, IFormFile ImageUrl, string Tags)
        {
            if (ImageUrl != null && ImageUrl.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ImageUrl.CopyToAsync(memoryStream);
                    pet.ImageUrl = memoryStream.ToArray();
                }

                // Remove the ModelState error for ImageUrl since we've handled it
                ModelState.Remove(nameof(pet.ImageUrl));
            }
            else
            {
                // If no file is uploaded, add a model error
                ModelState.AddModelError(nameof(pet.ImageUrl), "An image is required.");
            }

            // Handle Tags
            if (!string.IsNullOrEmpty(Tags))
            {
                var tagNames = Tags.Split(',').Select(t => t.Trim()).ToList();
                pet.Tags = new List<Tag>();

                foreach (var tagName in tagNames)
                {
                    var normalizedTagName = tagName.ToLower();
                    var existingTag = await _context.Tags
                        .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedTagName);

                    if (existingTag != null)
                    {
                        pet.Tags.Add(existingTag);
                    }
                    else
                    {
                        var newTag = new Tag { Name = tagName };
                        pet.Tags.Add(newTag);
                        _context.Tags.Add(newTag);
                    }
                }
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
            var pets = await _context.Pets.Include(p => p.Tags).Where(p => p.IsFavorite).ToListAsync();
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

            return RedirectToAction(nameof(Index));
        }

        // Filter pets (if implemented)
        public async Task<IActionResult> Filter(string species, int? minAge, int? maxAge, string breed)
        {
            var pets = _context.Pets.Include(p => p.Tags).AsQueryable();

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
