using System.ComponentModel.DataAnnotations;

namespace BasarSoftTask3_API.DTOs
{
    public class UserLoginRequestDTO
    {
        //[Required]
        public string Email { get; set; }
        //[Required]
        public string Password { get; set; }
    }
}
