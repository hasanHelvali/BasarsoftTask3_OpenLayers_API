using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly IConfiguration _configuraiton;
        private readonly UserManager<UserRegister> _userManager;
        public AuthManagementController(IConfiguration configuraiton, UserManager<UserRegister> userManager)
        {
            _configuraiton = configuraiton;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRegistrationRequestDTO userRegistrationRequestDTO)
        {
            var emailExist = await _userManager.FindByEmailAsync(userRegistrationRequestDTO.Email);
            if (emailExist != null)
                return BadRequest("Bu email sistemde kayıtlıdır. | Email already exist");
            
            //var newUsers = new IdentityUser()
            //{
            //    Email = userRegistrationRequestDTO.Email,
            //    UserName = userRegistrationRequestDTO.Name
            //};

            var isCreated = await _userManager.CreateAsync(new UserRegister 
                { Email=userRegistrationRequestDTO.Email,
                Name=userRegistrationRequestDTO.Name,
                UserName= userRegistrationRequestDTO.UserName,
                Password =userRegistrationRequestDTO.Password}
            );
            if (isCreated.Succeeded)
            {
                //Token Olusturma/Generate Token
                Token token = Services.TokenHandler.GenerateToken(_configuraiton);
                //Token Olusturma/Generate Token
                return Ok(token);
            }
            return BadRequest(error: isCreated.Errors.Select(x => x.Description).ToList());
        }
        [HttpGet]
        public async Task<IActionResult> GetToken()
        {
            Token token = Services.TokenHandler.GenerateToken(_configuraiton);
            return Ok(token);
        }

        //[HttpPost]
        //public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO userRegistrationRequestDTO)
        //{
        //    string str = _tokenService.GenerateToken();

        //    if (ModelState.IsValid)
        //    {
        //        var emailExist = await _userManager.FindByEmailAsync(userRegistrationRequestDTO.Email);
        //        if (emailExist != null)
        //            return BadRequest("email already exist");

        //        var newUser = new IdentityUser()
        //        {
        //            Email = userRegistrationRequestDTO.Email,
        //            UserName = userRegistrationRequestDTO.Email,
        //        };

        //        var isCreated = await _userManager.CreateAsync(newUser);
        //        if (isCreated.Succeeded)
        //        {
        //            //Token Olusturma/Generate Token
        //            return Ok(new RegistrationRequestResponse()
        //            {
        //                Result = true,
        //                Token = ""
        //            });
        //            return BadRequest(error: isCreated.Errors.Select(x => x.Description).ToList());
        //        }
        //        return BadRequest("Hata: Kullanıcı oluşturulurken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
        //        //Error:error creating the userü please try again later.
        //    }
        //    return BadRequest("Hata:Ilgili alanlar doldurulmadı.");//error: Invalid Request Payload
        //}

        //public async Task<IActionResult> GenerateJwtToken(IdentityUser identityUser)
        //{
        //    //var jwtTokenHandler=new JwtSecurityTokenHandler();
        //    //var key = Encoding.ASCII.GetBytes(_configuraiton.GetSection(key: "JwtConfig:Secret").Value);
        //    //var tokenDescriptor = new SecurityTokenDescriptor()
        //    //{
        //    //    Subject = new ClaimsIdentity(new[]
        //    //    {
        //    //        new Claim(type:"Id",value:identityUser.Id),
        //    //        new Claim(type:JwtRegisteredClaimNames.Sub,value:identityUser.Email),
        //    //        new Claim(type:JwtRegisteredClaimNames.Email,value:identityUser.Email),
        //    //        new Claim(type:JwtRegisteredClaimNames.Jti,value:Guid.NewGuid().ToString()),
        //    //    }),
        //    //    Expires = DateTime.UtcNow.AddMinutes(1),
        //    //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm: SecurityAlgorithms.HmacSha256)
        //    //};
        //    //var token=jwtTokenHandler.CreateToken(tokenDescriptor);
        //    //var jwtToken = jwtTokenHandler.WriteToken(token);
        //    //return jwtToken;
        //    //string str = _tokenService.GenerateToken();
        //    return Ok("");
        //}
    }
}
