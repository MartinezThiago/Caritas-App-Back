namespace CaritasBack.Models
{
    public class UsuarioRestablecer
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string ConfirmarClave { get; set; }
        public bool RestablecerClave { get; set; }
        public string Token { get; set; }
    }
}
