using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EBook_Proj.Models;
using Microsoft.Extensions.Options;

namespace EBook_Proj.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = isHtml;

                    using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }

        public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, string attachmentPath, bool isHtml = false)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = isHtml;

                    if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                    {
                        message.Attachments.Add(new Attachment(attachmentPath));
                    }

                    using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Failed to send email with attachment: {ex.Message}", ex);
            }
        }
    }
}