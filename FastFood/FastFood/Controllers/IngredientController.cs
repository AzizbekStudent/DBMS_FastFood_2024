using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    public class IngredientController : Controller
    {
        private readonly IRepository<Ingredients> _IngredientRepository;


        public IngredientController(IRepository<Ingredients> repository)
        {
            _IngredientRepository = repository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var ingredients = await _IngredientRepository.GetAllAsync();

                if (ingredients != null)
                    return View(ingredients);
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
                var ingredient = await _IngredientRepository.GetByIdAsync(id);

                if (ingredient != null) return View(ingredient);
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
        public async Task<IActionResult> Create(Ingredients ingredient, IFormFile image)
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
                        ingredient.Image = imageData;
                    }
                    else
                    {
                        ingredient.Image = null;
                    }

                    var product = new Ingredients
                    {
                        Title = ingredient.Title,
                        Price = ingredient.Price,
                        Amount_in_grams = ingredient.Amount_in_grams,
                        Unit = ingredient.Unit,
                        IsForVegan = ingredient.IsForVegan,
                        Image = ingredient.Image
                    };

                    int id = await _IngredientRepository.CreateAsync(product);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            return View(ingredient);
        }

        // Update
        public async Task<IActionResult> Edit(int id)
        {
            var ingredient = await _IngredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredients product, IFormFile image)
        {
            if (id != product.ingredient_ID)
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

                    var success = await _IngredientRepository.UpdateAsync(product);

                    if (success > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return BadRequest("Ingredient update failed.");
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
            var ingredient = await _IngredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var ingredient = await _IngredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            try
            {
                await _IngredientRepository.DeleteAsync(ingredient);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error {ex.Message}");
            }
        }

    }
}
