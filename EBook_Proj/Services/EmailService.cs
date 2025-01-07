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
        public async Task NotifyWaitingListUsersAsync(Books book, UserModel user)
        {
            try
            {
                string subject = "Good News - Your Requested Book is Available!";
                string body = $@"
            <h2>Dear {user.FirstName} {user.LastName},</h2>
            <p>We're excited to let you know that the book you've been waiting for is now available!</p>
            <div style='margin: 20px; padding: 15px; background-color: #f5f5f5; border-left: 4px solid #4CAF50;'>
                <h3 style='color: #2E7D32;'>{book.Title}</h3>
            </div>
            <p>You can now borrow this book from our digital library. Please note that this opportunity is available for the next 15 min's.</p>
            <p>If you don't borrow the book within this time frame, it will be offered to the next person in line.</p>
            <br>
            <p>Happy Reading!</p>
            <p style='color: #1976D2;'>Your EBook Store Team</p>
            <p style='font-size: 0.9em; color: #666;'>Don't forget - you have 15 min's to borrow this book!</p>";

                await SendEmailAsync(user.Email, subject, body, isHtml: true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to notify waiting list user: {ex.Message}", ex);
            }
        }
    }
}
