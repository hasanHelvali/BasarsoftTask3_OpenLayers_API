using BasarSoftTask3_API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeographyAuthorityController : ControllerBase
    {
        public GeographyAuthorityController()
        {
            
        }

        [HttpGet("{topologyId}")]
        public async Task<IActionResult> GetRoleWithUser(int topologyId )
        {

            return Ok();
        }

        [HttpPost("AssignRoleAndUser")]
        public async Task<IActionResult> AssignRoleAndUser(GeographyAuthority geographyAuthority)
        {
            return Ok();
        }

    }
}
