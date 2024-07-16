using System.Security.Claims;

namespace CaritasBack.Models
{
    public class Jwt
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Subject { get; set; }

        public static bool validarToken(ClaimsIdentity identity)
        {

            if(identity.Claims.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
