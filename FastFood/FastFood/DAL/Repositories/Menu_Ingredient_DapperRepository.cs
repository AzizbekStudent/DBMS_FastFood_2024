using Dapper;
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace FastFood.DAL.Repositories
{
    public class Menu_Ingredient_DapperRepository : IRepository<Menu_Ingredients>
    {
        private readonly string _connStr;

        public Menu_Ingredient_DapperRepository(string connStr)
        {
            _connStr = connStr;
        }

        // Main Functions
        // Create
        public Task<int> CreateAsync(Menu_Ingredients entity)
        {
            throw new NotImplementedException();
        }

        // Delete
        public Task<int> DeleteAsync(Menu_Ingredients entity)
        {
            throw new NotImplementedException();
        }

        // Get All
        public async Task<IEnumerable<Menu_Ingredients>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connStr);

            var results = await conn.QueryAsync<Menu_Ingredients>(
                "udp_Menu_Ingredients_Get_All",
                commandType: CommandType.StoredProcedure
            );

            var menuIngredients = new List<Menu_Ingredients>();

            foreach (var result in results)
            {
                var existingItem = menuIngredients.FirstOrDefault(mi => mi.meal_ID == result.meal_ID);

                if (existingItem == null)
                {
                    existingItem = new Menu_Ingredients
                    {
                        meal_ID = result.meal_ID,
                        Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                            "udp_Menu_Get_By_Id",
                            new { meal_ID = result.meal_ID },
                            commandType: CommandType.StoredProcedure
                        ),
                        IngredinetList = new List<Ingredients>()
                    };

                    menuIngredients.Add(existingItem);
                }

                var newIngredient = await conn.QueryFirstOrDefaultAsync<Ingredients>(
                    "udp_Ingredients_Get_By_Id",
                    new { ingredientID = result.ingredient_ID },
                    commandType: CommandType.StoredProcedure
                );

                existingItem.IngredinetList.Add(newIngredient);
            }

            return menuIngredients;
        }

        // Get By Id
        public async Task<Menu_Ingredients?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connStr);

            var meals = await conn.QueryAsync<Menu_Ingredients>(
                "udp_Menu_Ingredients_Get_By_Id",
                new { mealID = id },
                commandType: CommandType.StoredProcedure
                );

            var meal = new Menu_Ingredients();

            if (meals != null)
            {
                foreach (var m in meals)
                {
                    meal.Ingredient = await conn.QueryFirstOrDefaultAsync<Ingredients>(
                    "udp_Ingredients_Get_By_Id",
                    new { ingredientID = m.ingredient_ID },
                    commandType: CommandType.StoredProcedure
                    );

                    meal.Meal = await conn.QueryFirstOrDefaultAsync<Menu>(
                    "udp_Menu_Get_By_Id",
                    new { meal_ID = m.meal_ID },
                    commandType: CommandType.StoredProcedure
                    );

                    meal.IngredinetList.Add(meal.Ingredient);
                }

                return meal; 
            }


            return null;
        }

        // Update
        public Task<int> UpdateAsync(Menu_Ingredients entity)
        {
            throw new NotImplementedException();
        }
    }
}
