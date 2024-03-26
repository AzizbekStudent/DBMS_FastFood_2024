using FastFood.DAL.FilterResult;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;


namespace FastFood.Controllers
{
    // Students ID: 00013836, 00014725, 00014896
    public class EmployeeController : Controller
    {
        private readonly IRepository<Employee> _EmpRepository;

        private readonly IFilter_Employee _EmpFilter;

        private readonly I_Export_Employee _ExportEmployee;


        public EmployeeController(IRepository<Employee> repository, IFilter_Employee empFilter, I_Export_Employee exportEmployee)
        {
            _EmpRepository = repository;
            _EmpFilter = empFilter;
            _ExportEmployee = exportEmployee;
        }

        // Export To Json
        public IActionResult Export_To_Json(EmployeeFilterViewModel filter)
        {
            string json = _ExportEmployee.ExportTO_Json(filter.FName, filter.LName, filter.HireDate);
            return File(
                Encoding.UTF8.GetBytes(json),
                "application/json",
                $"Employees_{DateTime.Now}.json"
                );
        }

        
        // Export to XML
        public IActionResult Export_To_XML(EmployeeFilterViewModel filter)
        {
            string xml = _ExportEmployee.ExportTO_XML(filter.FName, filter.LName, filter.HireDate);

            return File(
                Encoding.UTF8.GetBytes(xml),
                "text/xml",
                $"Employees_{DateTime.Now}.xml"
                );
        }

        // Filtration
        public async Task<IActionResult> Filter(EmployeeFilterViewModel filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(filter.FName) || !string.IsNullOrEmpty(filter.LName) || filter.HireDate.HasValue || filter.PageSize > 0 || filter.PageNumber > 0)
                {

                    var (employees, totalCount) = await _EmpFilter.FilterEmployeesAsync(
                        filter.FName,
                        filter.LName,
                        filter.HireDate,
                        filter.SortField,
                        filter.SortAsc,
                        filter.PageNumber,
                        filter.PageSize
                                         
                        );

                    filter.Employees = employees;

                    int totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
                    filter.TotalCount = totalPages;

                    return View(filter);
                }
                else
                {
                   var (emp, totalCount)  =  await _EmpFilter.FilterEmployeesAsync(
                        filter.FName,
                        filter.LName,
                        filter.HireDate,
                        filter.SortField,
                        filter.SortAsc,
                        filter.PageNumber,
                        filter.PageSize

                        );
                    filter.Employees = emp;

                    int totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
                    filter.TotalCount = totalPages;

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
                if (image != null && image.Length > 0)
                {
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                    emp.Image = imageData;
                }
                else
                {
                    emp.Image = null;
                }


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
                    byte[] imageData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                    emp.Image = imageData;
                }
                else
                {
                    emp.Image = null;
                }


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
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(emp);
            }
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
