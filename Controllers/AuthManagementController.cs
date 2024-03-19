using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Feature.Attributes;
using BasarSoftTask3_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RTools_NTS.Util;
using System.Net;
using Token = BasarSoftTask3_API.Entities.Token;

namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {


        //Buranın acıklama satırlarını yazmamız gerekir.
        private readonly IConfiguration _configuraiton;
        private readonly UserManager<UserRegister> _userManager;
        private readonly SignInManager<UserRegister> _signInManager;
        public AuthManagementController(IConfiguration configuraiton, UserManager<UserRegister> userManager, SignInManager<UserRegister> signInManager)
        {
            _configuraiton = configuraiton;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("VerifyToken")]
        //[CustomHttp]
        public async Task<IActionResult> VerifyToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").TrimStart().TrimEnd();

            bool isValid = TokenHandler.JwtValidator(token);
            if (isValid)
            {
                return Ok();
            }
            else
            {
                throw new Exception("Token Geçerli Değil.");
            }
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(UserRegistrationRequestDTO userRegistrationRequestDTO)
        {
            var emailExist = await _userManager.FindByEmailAsync(userRegistrationRequestDTO.Email);
            var userNameExist = await _userManager.FindByNameAsync(userRegistrationRequestDTO.Email);
            if (emailExist != null)
                return BadRequest("Bu email sistemde kayıtlıdır. | Email already exist");
            else if (userNameExist != null)
                return BadRequest("Bu kullanıcı adı sistemde kayıtlıdır. | UserName already exist");

            var isCreated = await _userManager.CreateAsync(new UserRegister
            {
                Name = userRegistrationRequestDTO.Name,
                UserName = userRegistrationRequestDTO.Email,
                Password = userRegistrationRequestDTO.Password
            }

            );
            if (isCreated.Succeeded)
            {
         

                return Ok();
            }
            return BadRequest(error: isCreated.Errors.Select(x => x.Description).ToList());
        }
        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(UserLoginRequestDTO userLoginRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Geçersiz model.");
            }

            var user = await _userManager.FindByNameAsync(userLoginRequestDTO.Email);
            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı. Üye olun.");
            }

            if (user.UserName == userLoginRequestDTO.Email && user.Password == userLoginRequestDTO.Password)
            {
                var loginUser = await _userManager.FindByNameAsync(userLoginRequestDTO.Email);
                Token token = TokenHandler.GenerateToken(_configuraiton, loginUser.Name, loginUser.Id);
                return Ok(token);
            }
            else
            {
                return BadRequest("Geçersiz kullanıcı adı veya şifre.");
            }
            return Ok();
        }

        //[HttpGet]
        //public async Task<IActionResult> GetToken()
        //{
        //    return Ok();
        //}

    }
}
