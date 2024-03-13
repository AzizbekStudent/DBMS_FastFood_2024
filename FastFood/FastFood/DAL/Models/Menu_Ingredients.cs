namespace FastFood.DAL.Models
{
    // Students ID: 00013836, 00014725, 00014896
    public class Menu_Ingredients
    {
        // Junction Table
        // Foreign key
        public int? meal_ID { get; set; }
        public Menu? Meal { get; set; }

        // Foreign key
        public int? ingredient_ID { get; set; }
        public Ingredients? Ingredient { get; set; }

        public List<Ingredients> IngredinetList { get; set; } = new List<Ingredients>();

        public int? oldingredient_ID { get; set; }
    }
}
