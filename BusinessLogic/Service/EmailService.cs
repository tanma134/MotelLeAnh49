using System.Net;
using System.Net.Mail;
using BusinessLogic.Config;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Service
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        // ==============================
        // SEND OTP (AuthController dùng)
        // ==============================
        public void SendOTP(string toEmail, string otp)
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(_settings.Email);
            mail.To.Add(toEmail);
            mail.Subject = "Email Verification OTP";

            mail.IsBodyHtml = true;

            mail.Body = $@"
                <h2>Account Verification</h2>
                <p>Your OTP code is:</p>
                <h1 style='color:blue'>{otp}</h1>
                <p>This code expires in 5 minutes.</p>
            ";

            var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.Email,
                    _settings.Password
                ),
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        // ===================================
        // SEND BOOKING EMAIL (BookingController)
        // ===================================
        public void SendEmail(string toEmail, string subject, string body)
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(_settings.Email);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.Email,
                    _settings.Password
                ),
                EnableSsl = true
            };

            smtp.Send(mail);
        }
    }
}