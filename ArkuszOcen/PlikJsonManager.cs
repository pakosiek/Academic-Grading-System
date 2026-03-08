using ArkuszOcen.Model;
using System.IO;
using System.Text.Json;

namespace ArkuszOcen;

public static class PlikJsonManager
{
    public static bool Eksportuj(string plik, List<Student> lista)
    {
        try
        {
            string s = JsonSerializer.Serialize(lista);
            File.WriteAllText(plik, s);
            return true;
        }
        catch (Exception) { return false; }
    }

    public static List<Student> Importuj(string plik)
    {
        try
        {
            string s = File.ReadAllText(plik);
            List<Student> lista = JsonSerializer.Deserialize<List<Student>>(s)!;
            return lista;
        }
        catch (Exception) { return []; }
    }
}
