using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Feature.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<UserRegister> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<UserRegister> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpPut("UpdateUser")]
        [CustomHttp("SuperAdmin")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Users users)
        {
            var user = await _userManager.FindByIdAsync(users.ID);
            user.UserName = users.Name;
            user.Email = users.Email;
            await _userManager.UpdateAsync(user);
            return Ok(true);
        }

        [HttpDelete("{id}")]
        [CustomHttp("SuperAdmin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest("Böyle Bir Kullanıcı Bulunamadı.");
            }
            var result=await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(JsonSerializer.Serialize("Kullanıcı Başarıyla Silindi."));
            }
            else
            {
                return BadRequest(JsonSerializer.Serialize("Kullanıcı Silme İşlemi Sırasında Bir Hata Oluştu."));
            }
        }
    }
}
