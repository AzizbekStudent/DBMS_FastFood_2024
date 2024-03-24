using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Protocol.Core.Types;
using System.Text;
using System.Text.Json;

namespace FastFood.Controllers
{
    public class OrderController : Controller
    {
        private readonly IRepository<Order> _OrderRepository;
        private readonly IRepository<Menu> _MenuRepository;
        private readonly IRepository<Employee> _EmpRepository;

        private readonly I_ImportExport _ExportImportRepo;

        public OrderController(IRepository<Order> orderRepository, IRepository<Menu> menuRepository, IRepository<Employee> empRepository, I_ImportExport exportImportRepo)
        {
            _OrderRepository = orderRepository;
            _MenuRepository = menuRepository;
            _EmpRepository = empRepository;
            _ExportImportRepo = exportImportRepo;
        }

        public async Task<IActionResult> Index()
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

                var cookEmployees = employeeOptions.Where(e => e.Job == "cook").ToList();
                ViewBag.EmployeeOptions = cookEmployees.Select(e => new SelectListItem { Value = e.Employee_ID.ToString(), Text = e.FName }).ToList();

                var order = new Order
                {
                    OrderTime = DateTime.Now,
                    DeliveryTime = DateTime.Now,
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


                order.PaymentStatus = order.PaymentStatus;
                order.Meal_ID = order.Meal.Meal_ID;
                order.Amount = order.Amount;
                order.DeliveryTime = order.OrderTime.Add((TimeSpan)(order.Meal.TimeToPrepare * order.Amount));

                decimal totalPrice = order.Meal.Price * (decimal)order.Amount;

                order.TotalCost = totalPrice;

                order.Prepared_By = order.Staff.Employee_ID;

                int success = await _OrderRepository.CreateAsync(order);


                if(success > 0)    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(order);
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


            var employeeOptions = await _EmpRepository.GetAllAsync();
            if (employeeOptions == null)
            {
                employeeOptions = new List<Employee>();
            }

            var cookEmployees = employeeOptions.Where(e => e.Job == "cook").ToList();
            ViewBag.EmployeeOptions = cookEmployees.Select(e => new SelectListItem { Value = e.Employee_ID.ToString(), Text = e.FName }).ToList();


            var order = await _OrderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.order_ID)
            {
                return NotFound();
            }

            try
            {
                order.order_ID = order.order_ID;
                order.OrderTime = DateTime.Now;

                order.Meal = await _MenuRepository.GetByIdAsync((int)order.Meal_ID);
                order.Staff = await _EmpRepository.GetByIdAsync((int)order.Prepared_By);


                order.PaymentStatus = order.PaymentStatus;
                order.Meal_ID = order.Meal.Meal_ID;
                order.Amount = order.Amount;
                order.DeliveryTime = order.OrderTime.Add((TimeSpan)(order.Meal.TimeToPrepare * order.Amount));

                decimal totalPrice = order.Meal.Price * (decimal)order.Amount;
                order.TotalCost = totalPrice;
                order.Prepared_By = order.Staff.Employee_ID;



                var success = await _OrderRepository.UpdateAsync(order);

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
                return View(order);
            }

        }


        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            var ingredient = await _OrderRepository.GetByIdAsync(id);
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
            var ingredient = await _OrderRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            try
            {
                await _OrderRepository.DeleteAsync(ingredient);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error {ex.Message}");
            }
        }

        // Exporting to Json format
        public IActionResult ExportToJson()
        {
            string json = _ExportImportRepo.ExportOrderToJSON();
            return File(
                Encoding.UTF8.GetBytes(json),
                "application/json",
                $"Orders_{DateTime.Now}.json"
                );
        }

        // Importing From Json
        public IActionResult ImportFromJson()
        {
            return View(new List<Order>());
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromJson(IFormFile importFile)
        {
            if(importFile != null)
            {
                using var stream = importFile.OpenReadStream();
                using var reader = new StreamReader(stream);
                string json = await reader.ReadToEndAsync();

                var OrderList = await _ExportImportRepo.ImportFromJSON(json);
                return View(OrderList.ToList());
            }
            else
            {
                return View(new List<Order>());
            }
        }

        // Exporting into xml file
        public IActionResult ExportToXml()
        {
            string xml = _ExportImportRepo.ExportOrderToXML();
            return File(
                Encoding.UTF8.GetBytes(xml),
                "text/xml",
                $"Orders_{DateTime.Now}.xml"
                );
        }


        // Import From XML
        public IActionResult ImportFromXml()
        {
            return View(new List<Order>());
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromXml(IFormFile importFile)
        {
            try
            {
                if (importFile != null)
                {
                    using var stream = importFile.OpenReadStream();
                    using var reader = new StreamReader(stream);
                    string xml = await reader.ReadToEndAsync();

                    var OrderList = await _ExportImportRepo.ImportFromXml(xml);
                    return View(OrderList.ToList());
                }
                else
                {
                    return View(new List<Order>());
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return View(new List<Order>());
            }
        }
    }
}
