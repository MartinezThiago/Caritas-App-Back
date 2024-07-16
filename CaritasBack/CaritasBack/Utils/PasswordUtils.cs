using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace CaritasBack.Utils
{
    public class PasswordUtils
    {
        public static string ConvertirSHA256(string texto)
        {
            string hash = string.Empty;
        
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convierte el texto en un arreglo de bytes y calcula el hash
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));

                // Recorre cada byte del hash y lo convierte en un string hexadecimal
                foreach (byte b in bytes)
                {
                    hash += $"{b:X2}";
                }
            }
            return hash;
        }

        public static string GenerarToken()
        {
            string token = Guid.NewGuid().ToString("N");
            return token;
        }

        public static bool EsValida(string contrasenia)
        {
            // (?=.*[a-z]) The string must contain at least 1 lowercase alphabetical character
            // (?=.*[A-Z]) The string must contain at least 1 uppercase alphabetical character
            // (?=.*[0-9]) The string must contain at least 1 numeric character
            // (?=.*[!@#$%^&*]) The string must contain at least one special character, but we are escaping reserved RegEx characters to avoid conflict
            // (?=.{8,}) The string must be eight characters or longer
            Regex regex = new Regex(@"^(?=.*[!-~])(?=.{8,})");
            if (regex.IsMatch(contrasenia))
            {
                return true;
            }
            return false;

            /*if (contrasenia.Length < 6)
            {
                return false;
            }
            bool tieneMayuscula = false;
            bool tieneDigito = false;
            foreach (char c in contrasenia)
            {
                if (char.IsUpper(c))
                {
                    tieneMayuscula = true;
                }
                else if (char.IsDigit(c))
                {
                    tieneDigito = true;
                }
            }
            return tieneMayuscula && tieneDigito;*/
        }
    }
}

