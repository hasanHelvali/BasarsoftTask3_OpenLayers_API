using BasarSoftTask3_API.Context;
using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Feature.Attributes;
using BasarSoftTask3_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomHttp("SuperAdmin")]
    public class GeographyAuthorityController : ControllerBase
    {
        private readonly MapContext _mapContext;
        public GeographyAuthorityController(MapContext mapContext)
        {
            _mapContext = mapContext;
        }

        [HttpGet("{topologyId}")]
        public async Task<IActionResult> GetRoleWithUser(int topologyId)
        {

            return Ok();
        }

        [CustomHttp("SuperAdmin")]
        [HttpPost("AssignRoleAndUser")]
        //public async Task<IActionResult> AssignRoleAndUser(GeographyAuthority geographyAuthority)
        //public async Task<IActionResult> AssignRoleAndUser(GeoAuthDTO geoAuth)
        //{
        //    var geometry = GeometryAndWktConvert.WktToGeometrys(geoAuth.Topology.WKT);
        //    await _mapContext.GeographyAuthorities.AddAsync(new GeographyAuthority
        //    {
        //        LocAndUsers = new LocAndUsers
        //        {
        //            ID = geoAuth.Topology.ID,
        //            Name = geoAuth.Topology.Name,
        //            Type = geoAuth.Topology.Type,
        //            Geometry = geometry,
        //        },
        //        Users = new Users
        //        {
        //            ID = geoAuth.Users.ID,
        //            Name = geoAuth.Users.Name,
        //            Email = geoAuth.Users.Email,
        //            Role = geoAuth.Users.Role,
        //        },
        //        LocationID = geoAuth.Topology.ID,
        //        UsersID = geoAuth.Users.ID
        //    });
        //    await _mapContext.SaveChangesAsync();

        //    return Ok();
        //}

        public async Task<IActionResult> AssignRoleAndUser(GeoAuthDTO geoAuth)
        {
            //var geometry = GeometryAndWktConvert.WktToGeometrys(geoAuth.Topology.WKT);
            await _mapContext.GeographyAuthorities.AddAsync(new GeographyAuthority
            {
                LocationID = geoAuth.Topology.ID,
                UsersID = geoAuth.Users.ID,
            });
            await _mapContext.SaveChangesAsync();
            return Ok(true);
        }

        [HttpGet("GetActiveUser")]
        public async Task<IActionResult> GetActiveUser(int id)
        {
            var values = await _mapContext.GeographyAuthorities.ToListAsync();
            foreach (var value in values)
            {
                //value.
            }
            return Ok(values);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearTopology(string userId)
        {
            var clearTop = await _mapContext.GeographyAuthorities.Where(x => x.UsersID == userId).FirstOrDefaultAsync();
            if (clearTop != null)
            {
                _mapContext.GeographyAuthorities.Remove(clearTop);
                await _mapContext.SaveChangesAsync();
                return Ok(true);
            }
            else
            {
                return BadRequest("Bir Sorun Oluştu.");
            }
        }
    }
}
