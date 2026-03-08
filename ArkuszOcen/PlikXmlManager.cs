using ArkuszOcen.Model;
using System.IO;
using System.Xml.Serialization;

namespace ArkuszOcen;

public static class PlikXmlManager
{
    public static bool Eksportuj(string plik, List<Student> lista)
    {
        try
        {
            using FileStream fs = new(plik, FileMode.Create);
            XmlSerializer xs = new(typeof(List<Student>));
            xs.Serialize(fs, lista);
            return true;
        }
        catch (Exception) { return false; }
    }

    public static List<Student> Importuj(string plik)
    {
        try
        {
            using FileStream fs = new(plik, FileMode.Open);
            XmlSerializer xs = new(typeof(List<Student>));
            List<Student> lista = (List<Student>)xs.Deserialize(fs)!;
            return lista;
        }
        catch (Exception) { return []; }
    }
}
