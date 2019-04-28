using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace TECAirlines_WebAPI.Classes
{
    public class EmailHandler
    {
        /// <summary>
        /// Envía un email de confirmación al usuario.
        /// </summary>
        /// <param name="to_email">El email de destino.</param>
        public static void SendEmail(string to_email, string body)
        {
            var fromAddress = new MailAddress("m4ss97@gmail.com", "TEC Airlines");
            var toAddress = new MailAddress(to_email, "Client Reservation");
            const string fromPassword = "jijijijajaja516";
            const string subject = "Your Flight Reservation";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Construye el cuerpo del correo.
        /// </summary>
        /// <param name="res">Los datos de la reservación.</param>
        /// <returns></returns>
        public static string BuildEmailBody(string username, string flight, List<string> seats)
        {
            return "Thank you " + username + " for flying with TEC Airlines. You have successully pre checked flight "
                + flight + " with seats " + BuildEmailSeats(seats) + ". Enjoy your Flight.";
        }

        /// <summary>
        /// Construye una lista de asientos en formato string.
        /// </summary>
        /// <param name="seats">La lista de asientos.</param>
        /// <returns>La lista ens string.</returns>
        private static string BuildEmailSeats(List<string> seats)
        {
            string result = String.Empty;

            for(int i = 0; i < seats.Count; i++)
            {
                result += seats.ElementAt(i) + " ";
            }
            return result;
        }
    }
}