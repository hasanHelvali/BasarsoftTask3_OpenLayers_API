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
using NetTopologySuite.GeometriesGraph;
using NetTopologySuite.IO;
using System.Runtime.InteropServices;
namespace BasarSoftTask3_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[CustomHttp("SuperAdmin")]
    public class MapsController : ControllerBase
    {
        //private readonly IRepository<LocationAndUser> _repository;
        private readonly IMapRepository<LocAndUsers> _repository;
        private readonly MapContext _context;
        private readonly UserManager<UserRegister> _userManager;

        //public MapsController(IRepository<LocationAndUser> repository)
        public MapsController(IMapRepository<LocAndUsers> repository, MapContext context
            , UserManager<UserRegister> userManager
            )
        {
            _repository = repository;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ListLocation()
        {
            var _values = await _repository.GetAllAsync();
            var users = await _context.LocsAndUsers.Select(x => x.ID).ToListAsync();
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
                            .Select(z => z.UsersID)
                            .FirstOrDefaultAsync();
                value.UserId = userId;

                //var user = await _userManager.FindByIdAsync(userId);
                value.UserName = await _context.Users.Where(x => x.Id == userId).Select(x => x.UserName).FirstOrDefaultAsync();
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
            value.Geometry = geometry;
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
        public async Task<IActionResult> InteractionExists(PointIntersectionDTO pointIntersectionDTO)
        {
            //Burada userName e gore ilgili kaydı alıp o user icinde boyle bir alan tanımlı mı onu goruntulemeye calısıyoruz.
            var user = await _userManager.FindByIdAsync(pointIntersectionDTO.ID);//user alındı
            //ocnelikle user bulundu.
            var topologyWithUsers = await _context.GeographyAuthorities.Where(x => x.UsersID == user.Id).Select(y => new GeographyAuthority
            {
                ID = y.ID,
                LocationID = y.LocationID,
                UsersID = y.UsersID
            }).ToListAsync();
            //User aracılıgıyla ara tablodan ilgili user ile alakalı olan topolojiler alınır.
            List<LocAndUsers> locAndUser = null;
            LocAndUserDTO? locAndUserDTO = null;
            int sayac = 0;
            foreach (var topologyWithUser in topologyWithUsers)//Ilgili topolojiler uzerinde donulur.
            {
                sayac++;
                locAndUser = await _context.LocsAndUsers.Where(X => X.ID == topologyWithUser.LocationID).ToListAsync();
                //Ara tablodaki konum id leri ile LocAndUser daki id ler eslenir. Yani yetkilendirilmis konumlar cekilir.
                if (sayac == topologyWithUsers.Count && locAndUser == null)
                {
                    return Ok("Bir Kayıt Bulunamadı.");
                }
                var geom = GeometryAndWktConvert.WktToGeometrys(pointIntersectionDTO.PointWKT);//ilgili gelen wkt nin geometrisi elde edildi.
                locAndUserDTO = new LocAndUserDTO();

                locAndUserDTO = locAndUser.Where(x => x.Geometry.Contains(geom)).Select(y => new LocAndUserDTO
                {
                    ID = y.ID,
                    Name = y.Name,
                    Type = y.Type,
                    WKT = y.Geometry.ToText()
                }).FirstOrDefault();
                /*Gelen Wkt nin icinde bulundugu topoloji nesnesi tammanen alınıyor.*/

                if (locAndUserDTO != null)
                {
                    return Ok(locAndUserDTO); //Burası ilk dongu icin
                    //Bu nesne doluysa ilgili user a ait bir yetkilendirilmis konum var demektir. Bunu geri donuyoruz.
                }
            }
            if (locAndUserDTO != null)
                return Ok(locAndUserDTO);//Burası ilk donguden sonraki eslesmeler icindir.
                                         //Bu nesne doluysa ilgili user a ait bir yetkilendirilmis konum var demektir. Bunu geri donuyoruz.
            else
                //return Ok("Yetkiniz Yok.");//Burada ise bir alan yok demektir. Bos bir veri donuyorum.
                throw new Exception("Yetkiniz Yok");
        }
    }
}