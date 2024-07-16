using CaritasBack.Models;

namespace CaritasBack.Services
{
    public interface IAutenticarUsuario
    {
        //public UsuarioApi GetAutenticarUsuario(UsuarioLogin user);
        public Usuarios GetAutenticarUsuario(UsuarioLogin user);

    }
}
