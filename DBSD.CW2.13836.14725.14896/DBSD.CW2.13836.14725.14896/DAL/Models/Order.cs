namespace FastFood.DAL.Models
{
    // Students ID: 00013836, 00014725, 00014896
    public class Order
    {
        public int? order_ID { get; set; }

        public required DateTime OrderTime { get; set; }

        public required DateTime DeliveryTime { get; set; }

        public bool PaymentStatus { get; set; }

        // Foreign key
        public int? Meal_ID { get; set; }
        // getting object according to foreign key
        public Menu? Meal { get; set; }

        public int? Amount { get; set; } = 1;

        public decimal TotalCost { get; set; }

        // Foreign key
        public int? Prepared_By { get; set; }
        // getting object according to foreign key
        public Employee? Staff { get; set; }
    }
}
