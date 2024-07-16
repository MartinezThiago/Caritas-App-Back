using CaritasBack.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CaritasBack.Services
{
    public class Token : IValidarToken
    {
        private readonly string _secretKey;
        private readonly string _Issuer;
        private readonly string _Audience;
        //IOptions<> interfaz se utiliza para acceder a la configuración de la aplicación que ha sido vinculada a una clase específica mediante la configuración de opciones.
        public Token(IOptions<Jwt> appSettings)
        {
            _secretKey = appSettings.Value.SecretKey;
            _Issuer = appSettings.Value.Issuer;
            _Audience = appSettings.Value.Audience;
        }
        public bool validarToken(string token)
        {
            {
                string secretKey = _secretKey;
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    // Configura los parámetros de validación del token
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _Issuer,
                        ValidAudience = _Audience,
                        IssuerSigningKey = key
                    };

                    // Valida el token y devuelve el principal (claims principal) si es válido
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public string getToken(HttpContext httpContext)
        {
            return httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }
    }
}
