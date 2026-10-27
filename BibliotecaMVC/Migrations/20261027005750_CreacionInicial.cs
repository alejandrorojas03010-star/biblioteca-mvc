using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BibliotecaMVC.Migrations
{
    /// <inheritdoc />
    public partial class CreacionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Autor",
                columns: table => new
                {
                    IdAutor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nacionalidad = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autor", x => x.IdAutor);
                });

            migrationBuilder.CreateTable(
                name: "Socio",
                columns: table => new
                {
                    IdSocio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Socio", x => x.IdSocio);
                });

            migrationBuilder.CreateTable(
                name: "Libro",
                columns: table => new
                {
                    IdLibro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    IdAutor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libro", x => x.IdLibro);
                    table.CheckConstraint("CK_Libro_Stock", "[Stock] >= 0");
                    table.ForeignKey(
                        name: "FK_Libro_Autor_IdAutor",
                        column: x => x.IdAutor,
                        principalTable: "Autor",
                        principalColumn: "IdAutor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prestamo",
                columns: table => new
                {
                    IdPrestamo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSocio = table.Column<int>(type: "int", nullable: false),
                    IdLibro = table.Column<int>(type: "int", nullable: false),
                    FechaPrestamo = table.Column<DateTime>(type: "date", nullable: false),
                    FechaDevolucion = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamo", x => x.IdPrestamo);
                    table.CheckConstraint("CK_Prestamo_Fechas", "[FechaDevolucion] IS NULL OR [FechaDevolucion] >= [FechaPrestamo]");
                    table.ForeignKey(
                        name: "FK_Prestamo_Libro_IdLibro",
                        column: x => x.IdLibro,
                        principalTable: "Libro",
                        principalColumn: "IdLibro",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamo_Socio_IdSocio",
                        column: x => x.IdSocio,
                        principalTable: "Socio",
                        principalColumn: "IdSocio",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Autor",
                columns: new[] { "IdAutor", "Nacionalidad", "Nombre" },
                values: new object[,]
                {
                    { 1, "Argentina", "Jorge Luis Borges" },
                    { 2, "Argentina", "Julio Cortázar" },
                    { 3, "Colombiana", "Gabriel García Márquez" },
                    { 4, "Chilena", "Isabel Allende" },
                    { 5, "Argentina", "José Hernández" }
                });

            migrationBuilder.InsertData(
                table: "Socio",
                columns: new[] { "IdSocio", "Email", "FechaAlta", "Nombre" },
                values: new object[,]
                {
                    { 1, "laura.gomez@mail.com", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Laura Gómez" },
                    { 2, "martin.perez@mail.com", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martín Pérez" },
                    { 3, "sofia.diaz@mail.com", new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sofía Díaz" },
                    { 4, "carlos.ruiz@mail.com", new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carlos Ruiz" },
                    { 5, "ana.torres@mail.com", new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ana Torres" }
                });

            migrationBuilder.InsertData(
                table: "Libro",
                columns: new[] { "IdLibro", "Genero", "IdAutor", "Stock", "Titulo" },
                values: new object[,]
                {
                    { 1, "Cuento", 1, 3, "Ficciones" },
                    { 2, "Cuento", 1, 1, "El Aleph" },
                    { 3, "Novela", 2, 2, "Rayuela" },
                    { 4, "Cuento", 2, 0, "Bestiario" },
                    { 5, "Novela", 3, 4, "Cien años de soledad" },
                    { 6, "Novela", 3, 2, "Crónica de una muerte anunciada" },
                    { 7, "Novela", 4, 1, "La casa de los espíritus" },
                    { 8, "Poesía", 5, 5, "Martín Fierro" }
                });

            migrationBuilder.InsertData(
                table: "Prestamo",
                columns: new[] { "IdPrestamo", "FechaDevolucion", "FechaPrestamo", "IdLibro", "IdSocio" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2 },
                    { 3, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1 },
                    { 4, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3 },
                    { 5, new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 4 },
                    { 6, new DateTime(2026, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 2 },
                    { 7, new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5 },
                    { 8, null, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 3 },
                    { 9, null, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1 },
                    { 10, null, new DateTime(2026, 10, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 4 },
                    { 11, null, new DateTime(2026, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Libro_IdAutor",
                table: "Libro",
                column: "IdAutor");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamo_IdLibro",
                table: "Prestamo",
                column: "IdLibro");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamo_IdSocio",
                table: "Prestamo",
                column: "IdSocio");

            migrationBuilder.CreateIndex(
                name: "IX_Socio_Email",
                table: "Socio",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prestamo");

            migrationBuilder.DropTable(
                name: "Libro");

            migrationBuilder.DropTable(
                name: "Socio");

            migrationBuilder.DropTable(
                name: "Autor");
        }
    }
}
