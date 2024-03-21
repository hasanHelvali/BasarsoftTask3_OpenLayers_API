using BasarSoftTask3_API.Context;
using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Feature.Attributes;
using BasarSoftTask3_API.IRepository;
using BasarSoftTask3_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MapContext _mapContext;
        public AuthManagementController(IConfiguration configuraiton, UserManager<UserRegister> userManager
, RoleManager<IdentityRole> roleManager, MapContext mapContext)
        {
            _configuraiton = configuraiton;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapContext = mapContext;
        }

        [HttpGet("VerifyToken")]
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

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {

            //var usersAndRoles = await _mapContext.Users.Select(x=>new Users
            //{
            //    ID=x.Id,
            //    Name=x.UserName,
            //    Email=x.Email,
            //    Role=await _roleManager.Roles.Where(y => y.Id == x.Id).Select(y=>y.Name).FirstOrDefaultAsync(),
            //}).ToList();


            var usersAndRoles= await _mapContext.Users.Select(x => new Users
            {
                ID = x.Id,
                Name = x.UserName,
                Email = x.UserName,
                Role =  _mapContext.UserRoles.Where(ur => ur.UserId == x.Id).Select(x=>x.RoleId).ToList()//roleid
            }).ToListAsync();
            foreach (var user in usersAndRoles)
            {
                user.Role = await _mapContext.Roles
                    .Where(r => user.Role.Contains(r.Id))
                    .Select(r => r.Name)
                    .ToListAsync();
            }

            return Ok(usersAndRoles);
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
                Password = userRegistrationRequestDTO.Password,
            });

            //var roleExists = await _roleManager.RoleExistsAsync("SuperAdmin");
            //if (!roleExists)
            //{
            //    var role=new IdentityRole("User");
            //    await _roleManager.CreateAsync(role);
            //    var role2 = new IdentityRole("Admin");
            //    await _roleManager.CreateAsync(role2);
            //    var role3 = new IdentityRole("SuperAdmin");
            //    await _roleManager.CreateAsync(role3);
            //}

            if (isCreated.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(userRegistrationRequestDTO.Email);
                var role = new IdentityRole("User");
                await _userManager.AddToRoleAsync(user, role.Name);
                return Ok();//;
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
                var roles = await _userManager.GetRolesAsync(loginUser);
                Token token = TokenHandler.GenerateToken(_configuraiton, loginUser.Name, loginUser.Id, roles?.First());
                return Ok(token);
            }
            else
            {
                return BadRequest("Geçersiz kullanıcı adı veya şifre.");
            }
        }
    }
}
