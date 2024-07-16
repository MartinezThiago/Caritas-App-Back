
using CaritasBack.Models;
using System.Security.Claims;
namespace CaritasBack.Services
{
    public class CurrentUser : ICurrentUser
    {
        public Usuarios GetCurrentUser(ClaimsIdentity identity)
        {
            if (identity != null)
            {
                if (Jwt.validarToken(identity))
                {
                    var userClaims = identity.Claims;

                    return new Usuarios
                    {
                        nombre = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value,
                        mail = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value,
                        rol = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value,
                    };
                }

            }
            return null;

        }
    }
}
