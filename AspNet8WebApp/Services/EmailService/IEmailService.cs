
namespace AspNet8WebApp.Services.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string text);
    }
}