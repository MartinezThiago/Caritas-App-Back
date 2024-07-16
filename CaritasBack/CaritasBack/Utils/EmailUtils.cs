using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using CaritasBack.Models;

namespace CaritasBack.Utils
{
    public class EmailUtils
    {
        private static string _Host = "smtp.gmail.com";
        private static int _Puerto = 587;

        private static string _NombreEmisor = "InterCaritas";
        private static string _CorreoEmisor = "";
        // Contraseña de aplicación generada en la cuenta de Google
        private static string _ClaveEmisor = "";

        public static bool EnviarCorreo(CorreoModel correoModel)
        {
            try
            {
                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(_NombreEmisor, _CorreoEmisor));
                email.To.Add(new MailboxAddress("Usuario", correoModel.Para));              
                email.Subject = correoModel.Asunto;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = correoModel.Contenido
                };

                var smtp = new SmtpClient();
                smtp.Connect(_Host, _Puerto, SecureSocketOptions.StartTls);

                smtp.Authenticate(_CorreoEmisor, _ClaveEmisor);
                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
