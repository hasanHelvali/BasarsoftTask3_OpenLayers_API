using BasarSoftTask3_API.Context;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Feature.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Runtime.InteropServices;

namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("GetAllUsers")]
        [CustomHttp("Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var values = await _userManager.Users.Select(x=>new UserRegister () { Id=x.Id, UserName=x.Name, Email= x.Email }).ToListAsync();
            return Ok(values);
        }

        [HttpGet("GetRolesAndUsers")]
        [CustomHttp("Admin")]
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
        [CustomHttp("SuperAdmin")]
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
        [CustomHttp("SuperAdmin")]
        public async Task<IActionResult> UpdateRole(Users users)
        {
            Users _users = await _userManager.Users.Where(x=>x.Id==users.ID).Select(y=>new Users
            {
                ID=y.Id,
                Email=y.Email,
                Name=y.Name
            }).FirstOrDefaultAsync();

            var allRoles= await _mapContext.Roles.ToListAsync();
            var atanacakRol = allRoles.Find(x => x.Name == users.Role[0]);

            var  UserAndRole = await _mapContext.UserRoles.Where(x => x.UserId == users.ID).FirstOrDefaultAsync();
            _mapContext.UserRoles.Remove(UserAndRole);
            await _mapContext.SaveChangesAsync();
            UserAndRole.UserId = users.ID;
            UserAndRole.RoleId = atanacakRol.Id;
            await _mapContext.UserRoles.AddAsync(UserAndRole);
            await _mapContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole()
        {
            return Ok();
        }
    }

    public class A
    {
        public string Id { get; set; }
        public string Role { get; set; }
    }
}
