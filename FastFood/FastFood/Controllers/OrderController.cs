using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FastFood.Controllers
{
    public class OrderController : Controller
    {
        private readonly IRepository<Order> _OrderRepository;
        private readonly IRepository<Menu> _MenuRepository;
        private readonly IRepository<Employee> _EmpRepository;

        public OrderController(IRepository<Order> orderRepository, IRepository<Menu> menuRepository, IRepository<Employee> empRepository)
        {
            _OrderRepository = orderRepository;
            _MenuRepository = menuRepository;
            _EmpRepository = empRepository;
        }

        public async  Task<IActionResult> Index()
        {
            try
            {
                var orders = await _OrderRepository.GetAllAsync();

                if (orders != null)
                    return View(orders);
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
                var order = await _OrderRepository.GetByIdAsync(id);

                if (order != null) return View(order);
            }
            catch (Exception err)
            {
                return StatusCode(500, $"Server error {err.Message}");
            }
            return NotFound();
        }

        // Create
        public  async Task<IActionResult> Create()
        {
            try
            {
                var mealOptions = await _MenuRepository.GetAllAsync();

                if (mealOptions == null)
                {
                    mealOptions = new List<Menu>(); 
                }

                ViewBag.MealOptions = mealOptions.Select(m => new SelectListItem { 
                    Value = m.Meal_ID.ToString(), 
                    Text = $"{m.Meal_title} - Price: {m.Price}, Time to Prepare: {m.TimeToPrepare} minutes"
                }).ToList();


                var employeeOptions = await _EmpRepository.GetAllAsync();

                if (employeeOptions == null)
                {
                    employeeOptions = new List<Employee>();
                }
                ViewBag.EmployeeOptions = employeeOptions.Select(e => new SelectListItem { Value = e.Employee_ID.ToString(), Text = e.FName }).ToList();

                var order = new Order
                {
                    OrderTime = DateTime.Now,
                    DeliveryTime = DateTime.Now,
                    Amount = 1 
                };

                return View(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Error", "Home");
        }

        // Create method
        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            try
            {
                order.OrderTime = DateTime.Now;

                order.Meal = await _MenuRepository.GetByIdAsync((int)order.Meal_ID);
                order.Staff = await _EmpRepository.GetByIdAsync((int)order.Prepared_By);

                order.DeliveryTime = order.OrderTime.Add((TimeSpan)order.Meal.TimeToPrepare);

                order.PaymentStatus = order.PaymentStatus;
                order.Meal_ID = order.Meal.Meal_ID;
                order.Amount = order.Amount;

                decimal totalPrice = order.Meal.Price * (decimal)order.Amount;

                order.TotalCost = totalPrice;

                order.Prepared_By = order.Staff.Employee_ID;

                int id = await _OrderRepository.CreateAsync(order);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(order);
        }
    }
}
