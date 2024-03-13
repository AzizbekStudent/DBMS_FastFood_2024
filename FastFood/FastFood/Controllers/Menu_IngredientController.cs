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

        public Menu_IngredientController(IRepository<Menu_Ingredients> menuIngredientRepository)
        {
            _MenuIngredientRepository = menuIngredientRepository;
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
    }
}
