using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
namespace ArkuszOcen.Model;
public class Ocena {
    [Key]
    [XmlIgnore]
    [JsonIgnore]
    public int OcenaId { get; set; }
    public string? Przedmiot { get; set; }
    public double? Wartość { get; set; }
    [XmlIgnore]
    [JsonIgnore]
    [ForeignKey("StudentId")]
    public int StudentId { get; set; }
    [XmlIgnore]
    [JsonIgnore]
    public virtual Student? Student { get; set; }
    public Ocena() { }
    public override string ToString() => $"{Przedmiot}={Wartość}";
}