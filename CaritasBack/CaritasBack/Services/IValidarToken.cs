using CaritasBack.Models;

namespace CaritasBack.Services
{
    public interface IValidarToken
    {
        public bool validarToken(string token);
        public string getToken(HttpContext httpContext);
    }
}
