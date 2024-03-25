using FastFood.DAL.Models;

namespace FastFood.DAL.Interface
{
    // Students ID: 00013836, 00014725, 00014896
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
