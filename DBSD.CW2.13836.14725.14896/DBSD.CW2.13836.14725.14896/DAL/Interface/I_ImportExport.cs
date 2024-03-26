using FastFood.DAL.Models;

namespace FastFood.DAL.Interface
{
    // Students ID: 00013836, 00014725, 00014896
    public interface I_ImportExport
    {
        string ExportOrderToXML();

        string ExportOrderToJSON();

        Task<IEnumerable<Order>> ImportFromXml(string xml);

        Task<IEnumerable<Order>> ImportFromJSON(string json);
    }
}
