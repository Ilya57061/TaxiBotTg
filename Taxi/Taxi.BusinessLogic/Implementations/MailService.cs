
using System.Net;
using System.Net.Mail;
using Taxi.BusinessLogic.Interfaces;
using Taxi.Common.Models;

namespace Taxi.BusinessLogic.Implementations
{
    public class MailService : IMailService
    {
        public void Send(OrderModel order)
        {
            string smtpHost = "smtp.server.net";
            int smtpPort = 25;
            string login = "taxibottg@outlook.com";
            string pass = "dimasilyuha01";

            SmtpClient client = new SmtpClient(smtpHost, smtpPort);
            client.Credentials = new NetworkCredential(login, pass);

            string from = "taxibottg@outlook.com";
            string to = "taxibottg@outlook.com";
            string subject = $"{order.Phone}, {DateTime.Now}";
            string body = $"Телефон пассажира: {order.Phone}\nОт: {order.StartingAddress}\nДо: {order.DestinationAddress}";

            MailMessage mess = new MailMessage(from, to, subject, body);
            client.Send(mess);
        }
    }
}