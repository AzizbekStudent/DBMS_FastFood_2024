using Dapper;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using System.Data.SqlClient;
using System.Data;

namespace FastFood.DAL.Repositories
{
    public class OrderDapperRepository : IRepository<Order>
    {
        private readonly string _connStr;

        public OrderDapperRepository(string connStr)
        {
            _connStr = connStr;
        }

        // Create
        public async Task<int> CreateAsync(Order entity)
        {
            using var conn = new SqlConnection(_connStr);

            entity.Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                "udp_Menu_Get_By_Id",
                new { meal_ID = entity.Meal_ID },
                commandType: CommandType.StoredProcedure
                );

            entity.Staff = await conn.QueryFirstOrDefaultAsync<Employee>(
            "udp_Employee_Get_ByID",
            new { employeeID = entity.Prepared_By },
            commandType: CommandType.StoredProcedure
            );



            var parameters = new
            {
                entity.OrderTime,
                entity.DeliveryTime,
                entity.PaymentStatus,
                MealID = entity.Meal.Meal_ID,
                entity.Amount,
                entity.TotalCost,
                PreparedBy = entity.Staff.Employee_ID
            };

            return await conn.ExecuteAsync(
                "udp_Order_Create",
                parameters,
                commandType: CommandType.StoredProcedure
                );
        }

        // Delete
        public async Task<int> DeleteAsync(Order entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new
            {
                OrderID = entity.order_ID
            };

            return await conn.ExecuteAsync(
                "udp_Order_Delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // Get All
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connStr);

            var orderList = await conn.QueryAsync<Order>(
                "udp_Order_Get_All",
                commandType: CommandType.StoredProcedure);

            foreach (var order in orderList)
            {
                order.Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                "udp_Menu_Get_By_Id",
                new { meal_ID = order.Meal_ID },
                commandType: CommandType.StoredProcedure
                );

                order.Staff = await conn.QueryFirstOrDefaultAsync<Employee>(
                "udp_Employee_Get_ByID",
                new { employeeID = order.Prepared_By },
                commandType: CommandType.StoredProcedure
                );

                order.TotalCost = (order.Meal.Price * (decimal)order.Amount);
            }

            return orderList;
        }

        


        // Get By Id
        public async Task<Order?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connStr);

            var order = await conn.QueryFirstOrDefaultAsync<Order>(
                "udp_Order_Get_By_Id",
                new { OrderID = id },
                commandType: CommandType.StoredProcedure
                );
            order.Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                "udp_Menu_Get_By_Id",
                new { meal_ID = order.Meal_ID },
                commandType: CommandType.StoredProcedure
                );

            order.Staff = await conn.QueryFirstOrDefaultAsync<Employee>(
            "udp_Employee_Get_ByID",
            new { employeeID = order.Prepared_By },
            commandType: CommandType.StoredProcedure
            );

            order.TotalCost = (order.Meal.Price * (decimal)order.Amount);


            if (order != null)
                return order;

            return null;
        }

        // Update
        public async Task<int> UpdateAsync(Order entity)
        {
            using var conn = new SqlConnection(_connStr);

            entity.Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                "udp_Menu_Get_By_Id",
                new { meal_ID = entity.Meal_ID },
                commandType: CommandType.StoredProcedure
                );

            entity.Staff = await conn.QueryFirstOrDefaultAsync<Employee>(
            "udp_Employee_Get_ByID",
            new { employeeID = entity.Prepared_By },
            commandType: CommandType.StoredProcedure
            );

            var parameters = new DynamicParameters();

            parameters.Add("@OrderID", entity.order_ID);
            parameters.Add("@OrderTime", entity.OrderTime);
            parameters.Add("@DeliveryTime", entity.DeliveryTime);
            parameters.Add("@PaymentStatus", entity.PaymentStatus);
            parameters.Add("@MealID", entity.Meal.Meal_ID);
            parameters.Add("@Amount", entity.Amount);
            parameters.Add("@TotalCost", entity.TotalCost);
            parameters.Add("@PreparedBy", entity.Staff.Employee_ID);

            return await conn.ExecuteAsync(
                "udp_Order_Update",
                parameters,
                commandType: CommandType.StoredProcedure
                );
        }

       

    }
}
