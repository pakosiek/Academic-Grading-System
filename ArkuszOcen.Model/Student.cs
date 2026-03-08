using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
namespace ArkuszOcen.Model;
public class Student {
    [XmlIgnore]
    [JsonIgnore]
    [Key]
    public int StudentId { get; set; }
    public string? Imiê { get; set; }
    public string? Nazwisko { get; set; }
    public string? NumerIndeksu { get; set; }
    public string? Wydzia³ { get; set; }
    public List<Ocena> Oceny { get; set; }
    public Student() { }
    public override string ToString() =>
    $"{Imiê} {Nazwisko}/{NumerIndeksu}";
}