﻿using Dapper;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace FastFood.DAL.Repositories
{
    // Students ID: 00013836, 00014725, 00014896
    public class OrderDapperRepository : IRepository<Order>, I_ImportExport
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

                if(order.Meal != null)
                {
                    order.TotalCost = (order.Meal.Price * (decimal)order.Amount);
                }
                else
                {
                    order.TotalCost = 0;
                }
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
            if(order.Meal_ID != null)
            {
                order.Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                "udp_Menu_Get_By_Id",
                new { meal_ID = order.Meal_ID },
                commandType: CommandType.StoredProcedure
                );
            }
            else
            {
                order.Meal = null;
            }

            if(order.Prepared_By != null)
            {
               order.Staff = await conn.QueryFirstOrDefaultAsync<Employee>(
               "udp_Employee_Get_ByID",
               new { employeeID = order.Prepared_By },
               commandType: CommandType.StoredProcedure
               );
            }
            else
            {
                order.Staff = null;
            }

            if(order.Meal != null)
            {
                order.TotalCost = (order.Meal.Price * (decimal)order.Amount);

            }
            else
            {
                order.TotalCost = 0;
            }

            if (order != null)
                return order;

            return null;
        }

        // Update
        public async Task<int> UpdateAsync(Order entity)
        {
            using var conn = new SqlConnection(_connStr);

            // Preventing from null request
            if (entity.Meal_ID != null)
            {
                entity.Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
               "udp_Menu_Get_By_Id",
               new { meal_ID = entity.Meal_ID },
               commandType: CommandType.StoredProcedure
               );
            }
            else
            {
                entity.Meal = null;
            }

            // Preventing from null request
            if (entity.Prepared_By != null)
            {
                entity.Staff = await conn.QueryFirstOrDefaultAsync<Employee>(
                "udp_Employee_Get_ByID",
                new { employeeID = entity.Prepared_By },
                commandType: CommandType.StoredProcedure
                );
            }
            else
            {
                entity.Staff = null;
            }

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

        // Import from Json
        public async Task<IEnumerable<Order>> ImportFromJSON(string json)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                var orders = await conn.QueryAsync<Order>(
                    "udp_Order_Menu_Employee_Import_Json",
                    new { json = json },
                    commandType: CommandType.StoredProcedure);

                if (orders == null) return new List<Order>();
                foreach (var order in orders)
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
                }
                return orders.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return  new List<Order>(); ;
            }
        }

        // Import from Xml
        public async Task<IEnumerable<Order>> ImportFromXml(string xml)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                var orders = await conn.QueryAsync<Order>(
                    "udp_Order_Menu_Employee_Import_XML",
                    new { xml = xml },
                    commandType: CommandType.StoredProcedure);

                if (orders == null) return new List<Order>();
                foreach (var order in orders)
                {
                    order.Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                    "udp_Menu_Get_By_Id",
                    new { meal_ID = order.Meal_ID },
                    commandType: CommandType.StoredProcedure
                    );

                    order.TotalCost = (decimal)(order.Meal.Price * order.Amount);

                    order.Staff = await conn.QueryFirstOrDefaultAsync<Employee>(
                    "udp_Employee_Get_ByID",
                    new { employeeID = order.Prepared_By },
                    commandType: CommandType.StoredProcedure
                    );
                }

                return orders;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return new List<Order>();
            }
        }

        // Export to Json
        public string ExportOrderToJSON()
        {
            // For avoiding data cut I used
            // Query function then created string builder
            // Then I am concateneting each record
            using var conn = new SqlConnection(_connStr);
            var results = conn.Query<string>(
                "Export_Order_To_Json",
                commandType: CommandType.StoredProcedure);

            StringBuilder jsonStringBuilder = new StringBuilder();
            foreach (var jsonResult in results)
            {
                jsonStringBuilder.Append(jsonResult);
            }

            return jsonStringBuilder.ToString();
        }

        //Export to Xml
        public string ExportOrderToXML()
        {
            using var conn = new SqlConnection(_connStr);
            var results = conn.Query<string>(
                "Export_Order_To_Xml",
                commandType: CommandType.StoredProcedure);

            StringBuilder xmlStringBuilder = new StringBuilder();
            foreach (var xmlResult in results)
            {
                xmlStringBuilder.Append(xmlResult);
            }

            return xmlStringBuilder.ToString();
        }


    }
}
