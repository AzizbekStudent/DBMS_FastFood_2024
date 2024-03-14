using FastFood.DAL.Models;

namespace FastFood.DAL.FilterResult
{
    public class EmployeeFilterViewModel
    {
        public string? FName { get; set; }

        public string? LName { get; set; }

        public DateTime? HireDate { get; set; }

        public string SortField { get; set; } = "employee_ID";

        public bool SortAsc { get; set; } = true;

        public int PageNumber { get; set; }

        public int PageSize { get; set; } = 3;

        public IEnumerable<Employee> Employees { get; set; }

        public int? TotalCount { get; set; }
    }
}
