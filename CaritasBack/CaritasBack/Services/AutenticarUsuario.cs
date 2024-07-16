using CaritasBack.Models;
using CaritasBack.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace CaritasBack.Services
{
    public class AutenticarUsuario : IAutenticarUsuario
    {
    
        public Usuarios GetAutenticarUsuario(UsuarioLogin user)
        {
            List<Usuarios> usuariosDB = ServicioCaritasConsultas.Instance.listadDeUsuarios();
            var usuario = new Usuarios();
            usuario = usuariosDB.Where(x => x.mail.ToLower() == user.email.ToLower() && x.password.ToLower() == user.password).FirstOrDefault();
            return usuario;
        }

    }
}
