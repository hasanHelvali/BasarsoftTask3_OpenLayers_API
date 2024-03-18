using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BasarSoftTask3_API.DTOs
{
    public class UserRegistrationRequestDTO
    {
        //[Required]
        public string Name { get; set; }
        //[Required]
        public string Email { get; set; }
        //[Required]
        public string Password { get; set; }
    }
}
