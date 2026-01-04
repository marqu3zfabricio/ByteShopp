using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace CapaNegocio
{
    public class CN_Recursos
    {
        // =============================
        // MÉTODO: Convertir texto a SHA256
        // =============================
        public static string ConvertirSha256(string texto)
        {
            StringBuilder sb = new();
            byte[] result = SHA256.HashData(Encoding.UTF8.GetBytes(texto));

            foreach (byte b in result)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        // =============================
        // MÉTODO: Generar clave aleatoria
        // =============================
        public static string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N")[..6];
            return clave;
        }

        // =============================
        // MÉTODO: Enviar correo electrónico
        // =============================
        public static bool EnviarCorreo(string correo, string asunto, string mensaje)
        {
            bool resultado = false;

            try
            {
                using MailMessage mail = new();
                mail.To.Add(correo);
                mail.From = new MailAddress("byteshop974@gmail.com", "ByteShop - Notificaciones");
                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;

                using SmtpClient smtp = new("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("byteshop974@gmail.com", "eazs zgkz hqla zifj");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                resultado = true;
            }
            catch (Exception)
            {
                resultado = false;
            }

            return resultado;
        }
    }
}
