using Dapper;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using System.Data;
using System.Data.SqlClient;

namespace FastFood.DAL.Repositories
{
    public class EmployeeDapperRepository : IRepository<Employee>
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
    }
}
