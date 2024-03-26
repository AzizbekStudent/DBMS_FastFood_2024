using Dapper;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using System.Data.SqlClient;
using System.Data;

namespace FastFood.DAL.Repositories
{
    // Students ID: 00013836, 00014725, 00014896
    public class MenuDapperRepository : IRepository<Menu>
    {
        private readonly string _connStr;

        public MenuDapperRepository(string connStr)
        {
            _connStr = connStr;
        }

        // Main Functions
        // Create
        public async Task<int> CreateAsync(Menu entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new
            {
                entity.Meal_title,
                entity.Price,
                entity.Size,
                entity.TimeToPrepare,
                entity.IsForVegan,
                entity.Image,
                created_date = entity.Created_Date
            };

            return await conn.ExecuteAsync(
                "udp_Menu_Insert",
                parameters,
                commandType: CommandType.StoredProcedure
                );
        }


        // Delete
        public async Task<int> DeleteAsync(Menu entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new
            {
                meal_ID = entity.Meal_ID
            };

            return await conn.ExecuteAsync(
                "udp_Menu_Delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }


        // Get All
        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connStr);

            var MenuList = await conn.QueryAsync<Menu>(
                "udp_Menu_Get_All",
                commandType: CommandType.StoredProcedure);

            return MenuList;
        }


        // Get By Id
        public async Task<Menu?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connStr);

            var meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                "udp_Menu_Get_By_Id",
                new { meal_ID = id },
                commandType: CommandType.StoredProcedure
                );
            if (meal != null)
                return meal;

            return null;
        }


        // Update
        public async Task<int> UpdateAsync(Menu entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new DynamicParameters();

            parameters.Add("@meal_ID", entity.Meal_ID);
            parameters.Add("@meal_title", entity.Meal_title);
            parameters.Add("@price", entity.Price);
            parameters.Add("@size", entity.Size);
            parameters.Add("@TimeToPrepare", entity.TimeToPrepare);
            parameters.Add("@IsForVegan", entity.IsForVegan);
            parameters.Add("@Image", entity.Image, DbType.Binary);


            return await conn.ExecuteAsync(
                "udp_Menu_Update",
                parameters,
                commandType: CommandType.StoredProcedure
                );
        }
    }
}
