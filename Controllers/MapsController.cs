using BasarSoftTask3_API.Context;
using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Feature.Attributes;
using BasarSoftTask3_API.IRepository;
using BasarSoftTask3_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.IO;
using System.Runtime.InteropServices;
namespace BasarSoftTask3_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [CustomHttp("SuperAdmin")]
    public class MapsController : ControllerBase
    {
        //private readonly IRepository<LocationAndUser> _repository;
        private readonly IMapRepository<LocAndUsers> _repository;
        private readonly MapContext _context;
        //private readonly UserManager<IdentityUser> _userManager;

        //public MapsController(IRepository<LocationAndUser> repository)
        public MapsController(IMapRepository<LocAndUsers> repository, MapContext context
            //,UserManager<IdentityUser> userManager
            )
        {
            _repository = repository;
            _context = context;
            //_userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ListLocation()
        {
            var _values = await _repository.GetAllAsync();
            var users = await _context.LocsAndUsers.Select(x =>x.ID).ToListAsync();
            var values = _values.Select(x => new LocAndUserDTO
            {
                ID = x.ID,
                WKT = x.Geometry.ToText(),
                Name = x.Name,
                Type = x.Type,
            }).ToList();

            foreach (LocAndUserDTO value in values)
            {
                //UserId = await _context.GeographyAuthorities.Where(y => y.LocationID == x.ID).Select(z => z.UsersID).FirstOrDefaultAsync()
                var userId = await _context.GeographyAuthorities
                            .Where(y => y.LocationID == value.ID)
                            .Select(z => z.UsersID )
                            .FirstOrDefaultAsync();
                value.UserId = userId;

                //var user = await _userManager.FindByIdAsync(userId);
                value.UserName= await _context.Users.Where(x=>x.Id==userId).Select(x=>x.UserName).FirstOrDefaultAsync();
                //value.UserName = user?.UserName;
            }
            return Ok(values);
        }

        [HttpGet("LocationById/{id}")]

        public async Task<IActionResult> LocationById(int id)
        {
            var value = await _repository.GetByIdAsync(id);
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMap(LocAndUserDTO locAndUser)
        {
            var geometry = GeometryAndWktConvert.WktToGeometrys(locAndUser.WKT);
            await _repository.CreateAsync(new LocAndUsers
            {
                Name = locAndUser.Name,
                Type = locAndUser.Type,
                Geometry = geometry
            });
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateMap(LocAndUserDTO locAndUsers)
        {
            var value = await _repository.GetByIdAsync(locAndUsers.ID);
            var geometry = GeometryAndWktConvert.WktToGeometrys(locAndUsers.WKT);
            value.ID = locAndUsers.ID;
            value.Geometry = geometry ;
            value.Name = locAndUsers.Name;
            value.Type = locAndUsers.Type;

            await _repository.UpdateAsync(value);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var value = await _repository.GetByIdAsync(id);
            await _repository.RemoveAsync(value);
            return Ok();
        }
        [HttpPost("InteractionExists")]//Burada kontrol edilecek. 
        public async Task<IActionResult> InteractionExists( PointIntersectionDTO pointIntersectionDTO)
        {
            //Burada bu role e ve bu kullanıcı adınan ve bu lokasyına gore birn intersect tnaımlanabilir mi buna bakacaz.ç
           var geom = GeometryAndWktConvert.WktToGeometrys(pointIntersectionDTO.PointWKT);
            var _values = await _repository.GetAllAsync();
            var value = _values.Where(y=>y.Geometry.Contains(geom)).Select(x=>new LocAndUserDTO
            {
                ID=x.ID,
                Name=x.Name,
                Type=x.Type,
                WKT=x.Geometry.ToText()
            }).FirstOrDefault() ;
            return Ok(value);
        }
    }
}
