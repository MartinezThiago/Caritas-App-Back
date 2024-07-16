using Microsoft.AspNetCore.Mvc;
using CaritasBack.Models;
using CaritasBack.Services;

namespace CaritasBack.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly IAutenticarUsuario autenticado;
        private readonly IGenerateToken token;

        public LoginController(IGenerateToken token, IAutenticarUsuario autenticado)
        {
            this.autenticado = autenticado;
            this.token = token;
        }

        [HttpPost("[controller]/iniciarSesion")]
        public IActionResult IniciarSesion(UsuarioLogin user)
        {
            if (ModelState.IsValid) { 
            try
            {
                var userAutenticado = this.autenticado.GetAutenticarUsuario(user);

                if (userAutenticado != null)
                {
                    return Ok((this.token.GetToken(userAutenticado)));
                }
                return NotFound("El usuario no se encuentra en la base de datos");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            }
            else
            {
                return BadRequest("Datos incorrectos");
            }

        }

    }
}