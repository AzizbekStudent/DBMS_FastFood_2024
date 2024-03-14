using FastFood.DAL.Models;

namespace FastFood.DAL.Interface
{
    public interface IFilter_Employee
    {
        Task<(IEnumerable<Employee>, int)> FilterEmployeesAsync(
            string fName, 
            string lName, 
            DateTime? hireDate, 
            string sortField,
            bool sortAsc, 
            int pageNumber, 
            int pageSize);

    }
}
