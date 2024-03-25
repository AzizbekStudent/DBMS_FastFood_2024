using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    // Students ID: 00013836, 00014725, 00014896
    public class MenuController : Controller
    {
        private readonly IRepository<Menu> _MenuRepository;

        public MenuController(IRepository<Menu> menuRepository)
        {
            _MenuRepository = menuRepository;
        }

        // Get All
        public async Task<IActionResult> Index()
        {
            try
            {
                var menuList = await _MenuRepository.GetAllAsync();

                if (menuList != null)
                    return View(menuList);
            }
            catch (Exception err)
            {
                return StatusCode(500, $"Server error {err.Message}");
                throw;
            }
            return View();
        }

        // Get By Id
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var meal = await _MenuRepository.GetByIdAsync(id);

                if (meal != null) return View(meal);
            }
            catch (Exception err)
            {
                return StatusCode(500, $"Server error {err.Message}");
            }
            return NotFound();
        }

        // Create
        public IActionResult Create()
        {
            return View();
        }

        // Create method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Menu meal, IFormFile image)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                    meal.Image = imageData;
                }
                else
                {
                    meal.Image = null;
                }

              
                var product = new Menu
                {
                    Meal_title = meal.Meal_title,
                    Price = meal.Price,
                    Size = meal.Size,
                    TimeToPrepare = meal.TimeToPrepare,
                    IsForVegan = meal.IsForVegan,
                    Created_Date = DateTime.Now,
                    Image = meal.Image
                };

                int id = await _MenuRepository.CreateAsync(product);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(meal);
        }

        // Update
        public async Task<IActionResult> Edit(int id)
        {
            var meal = await _MenuRepository.GetByIdAsync(id);
            if (meal == null)
            {
                return NotFound();
            }
            return View(meal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Menu product, IFormFile image)
        {
            if (id != product.Meal_ID)
            {
                return NotFound();
            }

            try
            {
                if (image != null && image.Length > 0)
                {
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                    product.Image = imageData;
                }
                else
                {
                    product.Image = null;
                }

                
                var success = await _MenuRepository.UpdateAsync(product);

                if (success > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest("Meal update failed.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(product);
            }
        }

        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            var meal = await _MenuRepository.GetByIdAsync(id);
            if (meal == null)
            {
                return NotFound();
            }
            return View(meal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var meal = await _MenuRepository.GetByIdAsync(id);
            if (meal == null)
            {
                return NotFound();
            }

            try
            {
                await _MenuRepository.DeleteAsync(meal);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error {ex.Message}");
            }
        }

    }
}
