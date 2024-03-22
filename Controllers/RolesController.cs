using BasarSoftTask3_API.Context;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Feature.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomHttp]
    public class RolesController : ControllerBase
    {

        private readonly UserManager<UserRegister> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MapContext _mapContext;

        public RolesController(UserManager<UserRegister> userManager, RoleManager<IdentityRole> roleManager, MapContext mapContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapContext = mapContext;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var values = await _userManager.Users.Select(x=>new UserRegister () { Id=x.Id, UserName=x.Name, Email= x.Email }).ToListAsync();
            return Ok(values);
        }

        [HttpGet("GetRolesAndUsers")]
        public async Task<IActionResult> GetRolesAndUsers()
        {
            //var role = new IdentityRole("Admin");
            //var result = await _roleManager.CreateAsync(role);
            await _userManager.Users.Include(x => x.Id).Include(x => x.UserName).ToListAsync();
            //await _mapContext.Users.Include(x => x.);
            var usersWithRoles = await _mapContext.Users.Join(
                _mapContext.UserRoles,
                user => user.Id,
                 userRole => userRole.UserId,
                 (user, userRole) => new { User = user, UserRole = userRole }
                )
                .Join(
                    _mapContext.Roles,
                    userUserRole => userUserRole.UserRole.RoleId,
                    role => role.Id,
                    (userUserRole, role) => new
                    {
                        UserId = userUserRole.User.Id,
                        UserName = userUserRole.User.UserName,
                        Email = userUserRole.User.Email,
                        RoleName = role.Name
                    }
                ).ToListAsync();
            //Bu sorguya user in baska ozellikleir de eklenecek.
            return Ok(usersWithRoles);

            return Ok();
        }
        [HttpPost]
        [CustomHttp]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromForm] A a)
        {
            var user = await _userManager.FindByIdAsync(a.Id);
            if (user == null)
            {
                return BadRequest();
            }
            IdentityResult identityResult = await _userManager.AddToRoleAsync(user, a.Role);
            if (identityResult.Succeeded)
            {
                return Ok();
            }
            return BadRequest(error: identityResult.Errors.Select(x => x.Description).ToList());

            return Ok();
        }

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole(Users users)
        {
            //var user = await _userManager.FindByIdAsync(users.ID);

            //var usersAndRoles = await _mapContext.Users.Select(x => new Users
            //{
            //    ID = x.Id,
            //    Name = x.UserName,
            //    Email = x.UserName,
            //    Role = _mapContext.UserRoles.Where(ur => ur.UserId == x.Id).Select(x => x.RoleId).ToList()//roleid
            //}).ToListAsync();
            //foreach (var user in usersAndRoles)
            //{
            //    user.Role = await _mapContext.Roles
            //        .Where(r => user.Role.Contains(r.Id))
            //        .Select(r => r.Name)
            //        .ToListAsync();
            //}

            var a = await _mapContext.UserRoles.Select(X => X.UserId == users.ID).ToListAsync();//rollerde degil user larda don.
            
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole()
        {
            //await userManager.RemoveFromRoleAsync(user, "RolAdi");
            return Ok();
        }
    }

    public class A
    {
        public string Id { get; set; }
        public string Role { get; set; }
    }
}
