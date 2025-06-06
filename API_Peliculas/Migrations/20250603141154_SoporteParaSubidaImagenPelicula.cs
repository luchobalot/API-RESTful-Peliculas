﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Peliculas.Migrations
{
    /// <inheritdoc />
    public partial class SoporteParaSubidaImagenPelicula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RutaLocalImagen",
                table: "Pelicula",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RutaLocalImagen",
                table: "Pelicula");
        }
    }
}
