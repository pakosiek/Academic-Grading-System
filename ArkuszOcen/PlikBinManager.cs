using ArkuszOcen.Model;
using System.IO;

namespace ArkuszOcen;

public static class PlikBinManager
{
    public static bool Eksportuj(string plik, List<Student> lista)
    {
        try
        {
            using FileStream fs = new(plik, FileMode.Create);
            using BinaryWriter bw = new(fs);
            bw.Write(lista.Count);

            foreach (var student in lista)
            {
                bw.Write(student.Imię); // analogicznie pozostałe pola
                bw.Write(student.Nazwisko);
                bw.Write(student.NumerIndeksu);
                bw.Write(student.Wydział);
                bw.Write(student.Oceny.Count);
                foreach (var ocena in student.Oceny)
                {
                    bw.Write(ocena.Przedmiot); // analogicznie pozostałe pola
                    bw.Write((double)ocena.Wartość);
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
            int liczbaStudentow;
            int liczbaOcen;

            using FileStream fs = new(plik, FileMode.Open);
            using BinaryReader br = new(fs);
            liczbaStudentow = br.ReadInt32();

            for (int i = 0; i < liczbaStudentow; i++)
            {
                Student student = new()
                {
                    Imię = br.ReadString(), // analogicznie pozostałe pola
                    Nazwisko = br.ReadString(),
                    NumerIndeksu = br.ReadString(),
                    Wydział = br.ReadString(),
                    Oceny = []
                };

                liczbaOcen = br.ReadInt32();
                student.Oceny = [];

                for (int j = 0; j < liczbaOcen; j++)
                {
                    Ocena ocena = new()
                    {
                        Przedmiot = br.ReadString(), // analogicznie pozostałe pola
                        Wartość = br.ReadDouble()
                    };
                    student.Oceny.Add(ocena);
                }
                lista.Add(student);
            } // for i
            return lista;
        }
        catch (Exception) { return []; }
    }
}
