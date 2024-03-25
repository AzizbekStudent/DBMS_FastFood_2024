namespace FastFood.DAL.Interface
{
    // Students ID: 00013836, 00014725, 00014896
    public interface I_Export_Employee
    {
        string ExportTO_XML(
            string? fName,
            string? lName,
            DateTime? hireDate
            );

        string ExportTO_Json(
            string? fName,
            string? lName,
            DateTime? hireDate
            );
    }
}
