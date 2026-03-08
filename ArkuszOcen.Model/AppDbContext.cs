using Microsoft.EntityFrameworkCore;
namespace ArkuszOcen.Model;
public class AppDbContext : DbContext {
    private readonly string _parametryPołączenia =
    @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=ArkuszOcenNowy;Integrated Security=True";
    public DbSet<Student> Studenci { get; set; }
    public DbSet<Ocena> Oceny { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder oB) {
    oB.UseSqlServer(_parametryPołączenia);
    }
}