using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;

namespace FastFood.Controllers
{
    public class Menu_IngredientController : Controller
    {
        private readonly IRepository<Menu_Ingredients> _MenuIngredientRepository;
        private readonly IRepository<Menu> _MenuRepository;
        private readonly IRepository<Ingredients> _IngRepository;

        public Menu_IngredientController(IRepository<Menu_Ingredients> menuIngredientRepository, IRepository<Menu> MenuRepository, IRepository<Ingredients> IngRepository)
        {
            _MenuIngredientRepository = menuIngredientRepository;
            _IngRepository = IngRepository;
            _MenuRepository = MenuRepository;
        }

        public async  Task<IActionResult> Index()
        {

            var menuIngredients = await _MenuIngredientRepository.GetAllAsync();

            return View(menuIngredients);
        }

        // Get By Id
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var meal = await _MenuIngredientRepository.GetByIdAsync(id);

                if (meal != null) return View(meal);
            }
            catch (Exception err)
            {
                return StatusCode(500, $"Server error {err.Message}");
            }
            return NotFound();
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var mealOptions = await _MenuRepository.GetAllAsync();

                if (mealOptions == null)
                {
                    mealOptions = new List<Menu>();
                }

                ViewBag.MealOptions = mealOptions.Select(m => new SelectListItem
                {
                    Value = m.Meal_ID.ToString(),
                    Text = $"{m.Meal_title}"
                }).ToList();


                var ingredientOptions = await _IngRepository.GetAllAsync();
                if (ingredientOptions == null)
                {
                    ingredientOptions = new List<Ingredients>();
                }


                ViewBag.IngredientOptions = ingredientOptions.Select(i => new SelectListItem {
                    Value = i.ingredient_ID.ToString(),
                    Text = $"{i.Title}"
                }).ToList();


                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Error", "Home");
        }

        // Create method
        [HttpPost]
        public async Task<IActionResult> Create(Menu_Ingredients meal)
        {
            try
            {
                

                int success = await _MenuIngredientRepository.CreateAsync(meal);


                if (success > 0) return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(meal);
        }


    }
}
