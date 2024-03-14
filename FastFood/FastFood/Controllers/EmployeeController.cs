using FastFood.DAL.FilterResult;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using Microsoft.AspNetCore.Mvc;


namespace FastFood.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IRepository<Employee> _EmpRepository;

        private readonly IFilter_Employee _EmpFilter;


        public EmployeeController(IRepository<Employee> repository, IFilter_Employee empFilter)
        {
            _EmpRepository = repository;
            _EmpFilter = empFilter;
        }


        // Filtration
        // Filtration
        public async Task<IActionResult> Filter(EmployeeFilterViewModel filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(filter.FName) || !string.IsNullOrEmpty(filter.LName) || filter.HireDate.HasValue || filter.PageSize > 0 || filter.PageNumber > 0)
                {

                    var employees = await _EmpFilter.FilterEmployeesAsync(
                        filter.FName,
                        filter.LName,
                        filter.HireDate,
                        filter.SortField,
                        filter.SortAsc,
                        filter.PageNumber,
                        filter.PageSize
                                         
                        );

                    filter.Employees = employees;

                    return View(filter);
                }
                else
                {
                    filter.Employees = await _EmpRepository.GetAllAsync();
                }

                return View(filter);
            }
            catch (Exception err)
            {
                return StatusCode(500, $"Server error {err.Message}");
            }
        }



        // Get all implemented
        public async Task<IActionResult> Index()
        {
            try
            {
                var employees = await _EmpRepository.GetAllAsync();

                if (employees != null)
                    return View(employees);
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
                var employee = await _EmpRepository.GetByIdAsync(id);

                if (employee != null) return View(employee);
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
        public async Task<IActionResult> Create(Employee emp, IFormFile image)
        {
            try
            {
                byte[] imageData = null;
                if (image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                        emp.Image = imageData;
                    }
                }


                if (ModelState.IsValid)
                {
                    var employee = new Employee
                    {
                        FName = emp.FName,
                        LName = emp.LName,
                        Telephone = emp.Telephone,
                        Job = emp.Job,
                        Age = emp.Age,
                        Salary = emp.Salary,
                        HireDate = emp.HireDate,
                        Image = emp.Image,
                        FullTime = emp.FullTime
                    };

                    int id = await _EmpRepository.CreateAsync(employee);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(emp);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _EmpRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee emp, IFormFile image)
        {
            if (id != emp.Employee_ID)
            {
                return NotFound();
            }

            try
            {
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        emp.Image = memoryStream.ToArray();
                    }
                }


                if (ModelState.IsValid)
                {
                    var success = await _EmpRepository.UpdateAsync(emp);

                    if (success > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return BadRequest("Employee update failed.");
                    }
                }
                    
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(emp);
            }

            return View(emp);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _EmpRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var employee = await _EmpRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            try
            {
                await _EmpRepository.DeleteAsync(employee);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error {ex.Message}");
            }
        }
    }
    
}
