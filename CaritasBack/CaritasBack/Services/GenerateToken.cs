using CaritasBack.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CaritasBack.Services
{
    public class GenerateToken : IGenerateToken 
    {
        private readonly IConfiguration configuration;
        public GenerateToken(IConfiguration configuration){
            this.configuration = configuration;
        }
        public string GetToken(Usuarios user)
        {
            var jwt = this.configuration.GetSection("Jwt").Get<Jwt>();//bindiamos lo q hay en jwt del json a una clase.

            var claims = new[]//datos que encapsula el token 
            {
                    new Claim ("Nombre",user.nombre),
                    new Claim ("Apellido",user.apellido),
                    new Claim ("DNI",user.DNI),
                    new Claim ("Email",user.mail),
                    new Claim ("Fecha_Nacimiento",user.fecha_nacimiento.ToString()),
                    new Claim ("Centro",user.centro.ToString()),
                    new Claim ("userId",user.ID.ToString()),
                    new Claim ("Rol",user.rol),
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));//obtenemos key 
            var sigin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//firma token

            var token = new JwtSecurityToken( // creamos token en si
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: sigin
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
