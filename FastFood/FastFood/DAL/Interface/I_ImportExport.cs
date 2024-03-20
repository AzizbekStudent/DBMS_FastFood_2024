using FastFood.DAL.Models;

namespace FastFood.DAL.Interface
{
    public interface I_ImportExport
    {
        string ExportOrderToXML();

        string ExportOrderToJSON();

        IEnumerable<Order> ImportFromXml(string xml);

        IEnumerable<Order> ImportFromJSON(string json);
    }
}
