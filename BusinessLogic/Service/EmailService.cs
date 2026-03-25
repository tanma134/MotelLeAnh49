using System.Net;
using System.Net.Mail;
using BusinessLogic.Config;
using Microsoft.Extensions.Options;
using MotelLeAnh49.Models;

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
        // PRIVATE: CREATE SMTP CLIENT
        // ==============================
        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(
                    _settings.Email,
                    _settings.Password
                ),
                EnableSsl = true
            };
        }

        // ==============================
        // SEND OTP (AUTH)
        // ==============================
        public async Task SendOTPAsync(string toEmail, string otp)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_settings.Email, "🔐 Motel Le Anh"),
                Subject = "Email Verification OTP",
                IsBodyHtml = true,
                Body = $@"
                    <div style='font-family:Segoe UI'>
                        <h2>🔐 Account Verification</h2>
                        <p>Your OTP code is:</p>
                        <h1 style='color:#3498db'>{otp}</h1>
                        <p>This code expires in 5 minutes.</p>
                    </div>"
            };

            mail.To.Add(toEmail);

            using var smtp = CreateSmtpClient();
            await smtp.SendMailAsync(mail); // 🔥 async
        }

        // ==============================
        // GENERIC SEND EMAIL
        // ==============================
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_settings.Email, "🏨 Motel Le Anh"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            using var smtp = CreateSmtpClient();
            await smtp.SendMailAsync(mail); // 🔥 async
        }

        // ==============================
        // BUILD BOOKING TEMPLATE (ĐẸP)
        // ==============================
        public string BuildBookingTemplate(string title, Booking b, string color)
        {
            return $@"
            <div style='font-family:Segoe UI;background:#f5f7fa;padding:20px'>
                <div style='max-width:600px;margin:auto;background:white;padding:30px;border-radius:10px'>

                    <h2 style='text-align:center;color:#2c3e50'>{title}</h2>

                    <p>Xin chào <b>{b.FullName}</b>,</p>

                    <table style='width:100%;margin-top:15px'>
                        <tr><td><b>📞 Phone:</b></td><td>{b.Phone}</td></tr>
                        <tr><td><b>📧 Email:</b></td><td>{b.Email}</td></tr>
                        <tr><td><b>🏨 Room:</b></td><td>{b.RoomId}</td></tr>
                        <tr><td><b>📅 Check-in:</b></td><td>{b.CheckIn:dd/MM/yyyy}</td></tr>
                        <tr><td><b>📅 Check-out:</b></td><td>{b.CheckOut:dd/MM/yyyy}</td></tr>
                        <tr><td><b>👥 Guests:</b></td><td>{b.Adults} Adults, {b.Children} Children</td></tr>
                    </table>

                    <p style='margin-top:20px'>
                        Trạng thái:
                        <b style='color:{color};font-size:16px'>{b.Status}</b>
                    </p>

                    <hr/>

                    <p style='font-size:13px;color:#888;text-align:center'>
                        © Motel Le Anh System
                    </p>
                </div>
            </div>";
        }
    }
}