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
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;


namespace PetAdoptionApp.Controllers
{
    [Authorize]
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

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

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
            }
            else
            {
                ModelState.AddModelError("ImageUrl", "An image is required.");
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

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Search(PetSearchViewModel searchModel)
        {
            var query = _context.Pets.Include(p => p.Tags).AsQueryable();

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

            // Filter by tags
            if (!string.IsNullOrEmpty(searchModel.Tags))
            {
                var tagList = searchModel.Tags.Split(',').Select(t => t.Trim().ToLower()).ToList();
                query = query.Where(p => p.Tags.Any(tag => tagList.Contains(tag.Name.ToLower())));
            }

            // Execute the query and get the list of pets
            searchModel.Pets = await query.ToListAsync();

            return View(searchModel);
        }
    }
}
