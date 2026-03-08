using ArkuszOcen.Model;
using System.IO;

namespace ArkuszOcen;

public static class PlikTxtManager
{
    public static bool Eksportuj(string plik, List<Student> lista)
    {
        try
        {
            using FileStream fs = new(plik, FileMode.Create);
            using StreamWriter sw = new(fs);
            foreach (var s in lista)
            {
                sw.WriteLine(s.Imię); // analogicznie pozostałe pola
                sw.WriteLine(s.Nazwisko);
                sw.WriteLine(s.NumerIndeksu);
                sw.WriteLine(s.Wydział);
                sw.WriteLine(s.Oceny.Count);
                foreach (var o in s.Oceny)
                {
                    sw.WriteLine(o.Przedmiot); // analogicznie pozostałe pola
                    sw.WriteLine(o.Wartość);
                }
            } // foreach
            return true;
        }
        catch (Exception) { return false; }
    }

    public static List<Student> Importuj(string plik)
    {
        try
        {
            List<Student> lista = [];
            int liczbaOcen;
            using FileStream fs = new(plik, FileMode.Open);
            using StreamReader sr = new(fs);
            while (!sr.EndOfStream)
            {
                Student s = new()
                {
                    Imię = sr.ReadLine(), // analogicznie pozostałe pola
                    Nazwisko = sr.ReadLine(),
                    NumerIndeksu = sr.ReadLine(),
                    Wydział = sr.ReadLine(),
                    Oceny = []
                };
                liczbaOcen = int.Parse(sr.ReadLine());
                for (int i = 0; i < liczbaOcen; i++)
                {
                    Ocena o = new()
                    {
                        Przedmiot = sr.ReadLine(),
                        Wartość = double.Parse(sr.ReadLine()!)
                    };
                    s.Oceny.Add(o);
                }
                lista.Add(s);
            } // while
            return lista;
        }
        catch (Exception) { return []; }
    }
}
