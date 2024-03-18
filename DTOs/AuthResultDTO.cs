namespace BasarSoftTask3_API.DTOs
{
    public class AuthResultDTO
    {
        public string Token { get; set; }
        public bool Result{ get; set; }
        public List<string> Errors{ get; set; }
    }
}
