using CaritasBack.Models;

namespace CaritasBack.Services
{
    public interface IGenerateToken
    {
        public string GetToken(Usuarios user);

    }
}
