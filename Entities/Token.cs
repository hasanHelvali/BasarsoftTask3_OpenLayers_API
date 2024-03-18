namespace BasarSoftTask3_API.Entities
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }//Es gecilebilir
        public DateTime Expiration { get; set; }
    }
}
