using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Peliculas.Migrations
{
    /// <inheritdoc />
    public partial class CambiarTipoFechaEstreno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaEstreno",
                table: "Pelicula");

            migrationBuilder.AddColumn<int>(
                name: "AnioEstreno",
                table: "Pelicula",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnioEstreno",
                table: "Pelicula");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEstreno",
                table: "Pelicula",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
