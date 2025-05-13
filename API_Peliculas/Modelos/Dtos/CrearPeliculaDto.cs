namespace API_Peliculas.Modelos.Dtos
{
    public class CrearPeliculaDto
    {
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public int Duracion { get; set; }

        public string RutaImagen { get; set; }

        public enum CrearTipoClasificacion { Siete, Trece, Diesciseis, Dieciocho }

        public CrearTipoClasificacion Clasificacion { get; set; }

        public int AnioEstreno { get; set; }

        public int categoriaId { get; set; }


    }
}
