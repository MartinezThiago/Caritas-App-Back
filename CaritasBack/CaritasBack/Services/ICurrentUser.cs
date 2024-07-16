using CaritasBack.Models;
using System.Security.Claims;

namespace CaritasBack.Services
{
    public interface ICurrentUser
    {
        public Usuarios GetCurrentUser(ClaimsIdentity identity);
    }
}
