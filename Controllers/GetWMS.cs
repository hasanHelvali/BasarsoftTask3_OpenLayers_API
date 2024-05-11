using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetWMS : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public GetWMS(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetWms()
        {
            string baseUrl = _configuration["GeoServerUrl:GeoServerBaseUrl"];
            string wmsUrl = _configuration["GeoServerUrl:GeoServerGetWmsUrl"];
            string requestUrl = baseUrl + wmsUrl;

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                return File(stream, "image/png");
            }
            return NotFound();
        }
    }
}
