using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArkuszOcen.Model.Migrations
{
    /// <inheritdoc />
    public partial class MigracjaPierwsza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Studenci",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imię = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumerIndeksu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wydział = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studenci", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "Oceny",
                columns: table => new
                {
                    OcenaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Przedmiot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wartość = table.Column<double>(type: "float", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oceny", x => x.OcenaId);
                    table.ForeignKey(
                        name: "FK_Oceny_Studenci_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Studenci",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Oceny_StudentId",
                table: "Oceny",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Oceny");

            migrationBuilder.DropTable(
                name: "Studenci");
        }
    }
}
