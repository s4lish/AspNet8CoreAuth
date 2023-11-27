using System.ComponentModel.DataAnnotations;

namespace AspNet8WebApp.Data.Account
{
    public class SecretKey
    {
        [Key]
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
