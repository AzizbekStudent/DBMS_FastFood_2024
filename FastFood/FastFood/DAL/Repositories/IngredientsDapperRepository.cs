using Dapper;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using System.Data.SqlClient;
using System.Data;

namespace FastFood.DAL.Repositories
{
    // Students ID: 00013836, 00014725, 00014896
    public class IngredientsDapperRepository : IRepository<Ingredients>
    {
        private readonly string _connStr;

        public IngredientsDapperRepository(string connStr)
        {
            _connStr = connStr;
        }

        // Main functions
        // Create
        public async Task<int> CreateAsync(Ingredients entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new
            {
                entity.Title,
                entity.Price,
                entity.Amount_in_grams,
                entity.Unit,
                entity.IsForVegan,
                entity.Image
            };

            return await conn.ExecuteAsync(
                "udp_Ingredients_Insert",
                parameters,
                commandType: CommandType.StoredProcedure
                );
        }

        // Delete
        public async Task<int> DeleteAsync(Ingredients entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new
            {
                ingredientID = entity.ingredient_ID
            };

            return await conn.ExecuteAsync(
                "udp_Ingredients_Delete",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // Get All
        public async Task<IEnumerable<Ingredients>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connStr);

            var IngredientsList = await conn.QueryAsync<Ingredients>(
                "udp_Ingredients_Get_All",
                commandType: CommandType.StoredProcedure);

            return IngredientsList;
        }

        // Get by Id
        public async Task<Ingredients?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connStr);

            var ingredient = await conn.QueryFirstOrDefaultAsync<Ingredients>(
                "udp_Ingredients_Get_By_Id",
                new { ingredientID = id },
                commandType: CommandType.StoredProcedure
                );
            if (ingredient != null)
                return ingredient;

            return null;
        }

        // Update
        public async Task<int> UpdateAsync(Ingredients entity)
        {
            using var conn = new SqlConnection(_connStr);
            var parameters = new DynamicParameters();

            parameters.Add("@ingredientID", entity.ingredient_ID);
            parameters.Add("@Title", entity.Title);
            parameters.Add("@Price", entity.Price);
            parameters.Add("@Amount_in_grams", entity.Amount_in_grams);
            parameters.Add("@Unit", entity.Unit);
            parameters.Add("@IsForVegan", entity.IsForVegan);
            parameters.Add("@Image", entity.Image, DbType.Binary);


            return await conn.ExecuteAsync(
                "udp_Ingredients_Update",
                parameters,
                commandType: CommandType.StoredProcedure
                );
        }
    }
}
