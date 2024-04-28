using SQLite;

namespace mercadito.LocalDbModels
{
    [Table("cart")]
    public class Cart
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("imagen")]
        public string Imagen { get; set; }

        [Column("precio")]
        public int Precio { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }
    }
}
