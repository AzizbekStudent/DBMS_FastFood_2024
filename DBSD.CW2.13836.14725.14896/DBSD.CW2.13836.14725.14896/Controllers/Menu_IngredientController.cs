using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;

namespace FastFood.Controllers
{
    // Students ID: 00013836, 00014725, 00014896
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

                var meal = new Menu_Ingredients();
                return View(meal);
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
                if (meal.meal_ID == null || meal.ingredient_ID == null)
                {
                    ModelState.AddModelError(string.Empty, "Cannot submit empty form");
                    return View(meal); 
                }

                int success = await _MenuIngredientRepository.CreateAsync(meal);


                if (success > 0) return RedirectToAction("Index");
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

            var mealOptions = await _MenuRepository.GetAllAsync();

            if (mealOptions == null)
            {
                mealOptions = new List<Menu>();
            }

            ViewBag.MealOptions = mealOptions.Select(m => new SelectListItem
            {
                Value = m.Meal_ID.ToString(),
                Text = $"{m.Meal_title} - Price: {m.Price}, Time to Prepare: {m.TimeToPrepare} minutes"
            }).ToList();

            var ingredientOptions = await _IngRepository.GetAllAsync();
            if (ingredientOptions == null)
            {
                ingredientOptions = new List<Ingredients>();
            }


            ViewBag.IngredientOptions = ingredientOptions.Select(i => new SelectListItem
            {
                Value = i.ingredient_ID.ToString(),
                Text = $"{i.Title}"
            }).ToList();



            var ChosenMeal = await _MenuIngredientRepository.GetByIdAsync(id);
            if (ChosenMeal == null)
            {
                return NotFound();
            }


            return View(ChosenMeal);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Menu_Ingredients meal)
        {

            if (meal.meal_ID == null || meal.ingredient_ID == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot submit empty form");
                return View(meal); 
            }

            if (id != meal.meal_ID)
            {
                return NotFound();
            }

            try
            {
                var newMeal = new Menu_Ingredients()
                {
                    meal_ID = id,

                    oldingredient_ID = meal.oldingredient_ID,

                    ingredient_ID = meal.ingredient_ID

                };

                var success = await _MenuIngredientRepository.UpdateAsync(newMeal);

                if (success > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest("Order update failed.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(meal);
            }

        }


        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            var meal = await _MenuIngredientRepository.GetByIdAsync(id);
            if (meal == null)
            {
                return NotFound();
            }
            return View(meal);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(int mealId, int ingredientId)
        {
            var meal = await _MenuIngredientRepository.GetByIdAsync(mealId);
            if (meal == null)
            {
                return NotFound();
            }

            try
            {
                meal.ingredient_ID = ingredientId;
                await _MenuIngredientRepository.DeleteAsync(meal);

                var MEAL_ = await _MenuIngredientRepository.GetByIdAsync(mealId);
                if (MEAL_ != null)
                    return RedirectToAction("Delete", new { id = mealId });
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Delete", new { id = mealId });
            }
        }

    }
}
