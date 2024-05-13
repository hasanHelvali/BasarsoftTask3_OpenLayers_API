using BasarSoftTask3_API.DTOs;
using BasarSoftTask3_API.Entities;
using BasarSoftTask3_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using NetTopologySuite.IO;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetWFSController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetWFSController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet("GetWfsById/{id}")]
        public async Task<IActionResult> GetWfsById(int id)
        {
            string baseUrl = _configuration["GeoServerUrl:GeoServerBaseWfsUrl"];
            string wfsUrl = _configuration["GeoServerUrl:GeoServerGetWfsUrl"];
            string wfsUrlById = string.Format(baseUrl + wfsUrl, id);//id parametresi appsetting json dan gelen url e parametrik olarak gecildi.

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(wfsUrlById);
            if (response.IsSuccessStatusCode)
            {
                var geoJsonData = await response.Content.ReadAsStringAsync();//response icerigi okunur
                //var feature = JsonConvert.DeserializeObject<>(geoJsonData);
                //var reader = new GeoJsonReader();
                //FeatureCollection featureCollection = reader.Read<FeatureCollection>(geoJsonData);
                //try
                //{
                ////var feature = reader.Read<DTOs.Feature>(geoJsonData);
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception(ex.Message);
                //}
                return Ok(geoJsonData);
            }
            else
            {
                return BadRequest();
            }

        }
    }
}
//