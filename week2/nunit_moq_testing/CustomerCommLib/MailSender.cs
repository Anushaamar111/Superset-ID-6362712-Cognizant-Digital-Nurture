using System.Net;
using System.Net.Mail;

namespace CustomerCommLib
{
    /// <summary>
    /// Interface for mail sending functionality
    /// This interface allows us to create mock objects for testing
    /// </summary>
    public interface IMailSender
    {
        bool SendMail(string toAddress, string message);
    }

    /// <summary>
    /// Real implementation of mail sender that communicates with SMTP server
    /// This class would be difficult to unit test without mocking
    /// </summary>
    public class MailSender : IMailSender
    {
        public bool SendMail(string toAddress, string message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("your_email_address@gmail.com");
                mail.To.Add(toAddress);
                mail.Subject = "Test Mail";
                mail.Body = message;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential("username", "password");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Customer communication class that uses dependency injection
    /// This design makes the class testable by allowing us to inject mock dependencies
    /// </summary>
    public class CustomerComm
    {
        private readonly IMailSender _mailSender;

        // Constructor injection - the dependency is injected through the constructor
        public CustomerComm(IMailSender mailSender)
        {
            _mailSender = mailSender;
        }

        /// <summary>
        /// Business logic method that sends mail to customers
        /// This method can be unit tested by mocking the IMailSender dependency
        /// </summary>
        public bool SendMailToCustomer()
        {
            // Actual business logic would go here
            // For demonstration, we're using hardcoded values
            string customerEmail = "cust123@abc.com";
            string message = "Some Message";

            // Call the injected dependency
            bool result = _mailSender.SendMail(customerEmail, message);
            
            return result;
        }

        /// <summary>
        /// Overloaded method that accepts custom email and message
        /// </summary>
        public bool SendMailToCustomer(string email, string message)
        {
            return _mailSender.SendMail(email, message);
        }
    }
}
