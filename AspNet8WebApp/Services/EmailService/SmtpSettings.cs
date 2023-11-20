namespace AspNet8WebApp.Services.EmailService
{
    public class SmtpSettings
    {
        public string MAIL_HOST { get; set; } = string.Empty;
        public int MAIL_PORT { get; set; } = 0;
        public string MAIL_USERNAME { get; set; } = string.Empty;
        public string MAIL_PASSWORD { get; set; } = string.Empty;
        public string MAIL_ADDRESS { get; set; } = string.Empty;

    }
}
