using EBook_Proj.Models;

namespace EBook_Proj.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false);
    Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, string attachmentPath, bool isHtml = false);
    Task NotifyWaitingListUsersAsync(Books book, UserModel user);

}