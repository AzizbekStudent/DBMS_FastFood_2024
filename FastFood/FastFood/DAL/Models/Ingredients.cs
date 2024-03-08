namespace FastFood.DAL.Models
{
    // Students ID: 00013836, 00014725, 00014896
    public class Ingredients
    {
        public int? ingredient_ID { get; set; }

        public required string Title { get; set; }

        public required decimal Price { get; set; }

        public int? Amount_in_grams { get; set; }

        public int? Unit { get; set; }

        public bool? IsForVegan { get; set; }

        public byte[]? Image { get; set; }
    }
}
