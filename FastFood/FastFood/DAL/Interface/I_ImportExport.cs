using FastFood.DAL.Models;

namespace FastFood.DAL.Interface
{
    public interface I_ImportExport
    {
        string ExportOrderToXML();

        string ExportOrderToJSON();

        Task<IEnumerable<Order>> ImportFromXml(string xml);

        Task<IEnumerable<Order>> ImportFromJSON(string json);
    }
}
