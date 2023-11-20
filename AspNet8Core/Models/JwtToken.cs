namespace AspNet8Core.Models
{
    public class JwtToken
    {
        public string access_toekn { get; set; } = string.Empty;
        public DateTime expires_at { get; set; }
    }
}
