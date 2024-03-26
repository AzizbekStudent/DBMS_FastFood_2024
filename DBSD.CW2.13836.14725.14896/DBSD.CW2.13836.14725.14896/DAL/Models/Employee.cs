namespace FastFood.DAL.Models
{
    // Students ID: 00013836, 00014725, 00014896
    public class Employee
    {
        public int? Employee_ID { get; set; }

        public required string FName { get; set; }

        public required string LName { get; set; }

        public string? Telephone { get; set; }

        public required string Job { get; set; }

        public int? Age { get; set; }

        public decimal? Salary { get; set; }

        public DateTime? HireDate { get; set; }

        public byte[]? Image { get; set; }

        public required bool FullTime { get; set; }
    }
}
