using Dapper;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using System.Data;
using System.Data.SqlClient;

namespace FastFood.DAL.Repositories
{
    public class EmployeeDapperRepository : IRepository<Employee>, IFilter_Employee
    {
        private readonly string _connStr;

        public EmployeeDapperRepository(string connStr)
        {
            _connStr = connStr;
        }

        // Main methods
        // Create
        public async Task<int> CreateAsync(Employee entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new
            {
                entity.FName,
                entity.LName,
                entity.Telephone,
                entity.Job,
                entity.Age,
                entity.Salary,
                entity.HireDate,
                entity.Image,
                entity.FullTime
            };

            return await conn.ExecuteAsync(
                "udp_Employee_Insert",
                parameters,
                commandType: CommandType.StoredProcedure
                );
        }

        // Delete
        public async Task<int> DeleteAsync(Employee entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new
            {
                employeeID = entity.Employee_ID
            };

            return await conn.ExecuteAsync(
                "udp_Employee_Delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }


        // Get All
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connStr);

            var EmpList = await conn.QueryAsync<Employee>(
                "udp_GetAllEmployee",
                commandType: CommandType.StoredProcedure);

            return EmpList;
        }

        // Get By ID
        public async Task<Employee?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connStr);

            var employee = await conn.QueryFirstOrDefaultAsync<Employee>(
                "udp_Employee_Get_ByID",
                new { employeeID = id},
                commandType: CommandType.StoredProcedure
                );
            if(employee != null)
                return employee;

            return null;
        }

        // Update
        public async Task<int> UpdateAsync(Employee entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new DynamicParameters();

            parameters.Add("@employeeID", entity.Employee_ID);
            parameters.Add("@FName", entity.FName);
            parameters.Add("@LName", entity.LName);
            parameters.Add("@Telephone", entity.Telephone);
            parameters.Add("@Job", entity.Job);
            parameters.Add("@Age", entity.Age);
            parameters.Add("@Salary", entity.Salary);
            parameters.Add("@HireDate", entity.HireDate);
            parameters.Add("@Image", entity.Image);
            parameters.Add("@FullTime", entity.FullTime);


            return await conn.ExecuteAsync(
                "udp_Employee_Update",
                parameters,
                commandType: CommandType.StoredProcedure
                );
        }

        // Filter
        public async Task<(IEnumerable<Employee>, int)> FilterEmployeesAsync(
            string fName, 
            string lName, 
            DateTime? hireDate, 
            string sortField, 
            bool sortAsc, 
            int pageNumber,
            int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 3;

            using var conn = new SqlConnection(_connStr);

            var parameters = new DynamicParameters();
            parameters.Add("FName", fName);
            parameters.Add("LName", lName);
            parameters.Add("HireDate", hireDate);
            parameters.Add("SortField", sortField ?? "employee_ID");
            parameters.Add("SortAsc", sortAsc);
            parameters.Add("PageNumber", pageNumber);
            parameters.Add("PageSize", pageSize);
            parameters.Add("@TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);


            var employees =  await conn.QueryAsync<Employee>(
                "udp_Filter_Employee",
                parameters,
                commandType: CommandType.StoredProcedure);

            var totalCount = parameters.Get<int>("@TotalCount");

            return (employees, totalCount);
        }


    }
}
