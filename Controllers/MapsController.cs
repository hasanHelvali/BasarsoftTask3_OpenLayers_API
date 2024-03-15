using BasarSoftTask2_API.DTOs;
using BasarSoftTask2_API.Entities;
using BasarSoftTask2_API.IRepository;
using BasarSoftTask2_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.IO;
using System.Runtime.InteropServices;
namespace BasarSoftTask2_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        //private readonly IRepository<LocationAndUser> _repository;
        private readonly IMapRepository<LocAndUsers> _repository;

        //public MapsController(IRepository<LocationAndUser> repository)
        public MapsController(IMapRepository<LocAndUsers> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> ListLocation()
        {
            var _values = await _repository.GetAllAsync();
            var values = _values.Select(x => new LocAndUserDTO
            {
                ID = x.ID,
                WKT=x.Geometry.ToText(),
                Name = x.Name,
                Type = x.Type,
            }).ToList();

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


        [HttpPost("InteractionExists")]
        public async Task<IActionResult> InteractionExists( PointIntersectionDTO pointIntersectionDTO)
        {
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
