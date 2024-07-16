//usuarios que simulan ser los de la DB
namespace CaritasBack.Models
{
    public partial class Usuarios
    {
        public int ID { get; set; }
        public string nombre { get; set; } 
        public string apellido { get; set; }
        public string DNI { get; set; }
        public string mail { get; set; }
        public string password { get; set; } 
        public string fecha_registro { get; set; }
        public string fecha_nacimiento { get; set; }
        public string rol { get; set; }
        public int centro { get; set; }
    }
    public partial class UsuariosViewModel 
    {
        public int ID { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string fecha_registro { get; set; }
        public string fecha_nacimiento { get; set; }
        public string DNI { get; set; }
        public string mail { get; set; }
        public string foto { get; set; }
        public string rol { get; set; }
        public int centro { get; set; }
        public bool borrado { get; set; }
    }
}
