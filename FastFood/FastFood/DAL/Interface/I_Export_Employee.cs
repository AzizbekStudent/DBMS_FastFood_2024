namespace FastFood.DAL.Interface
{
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
