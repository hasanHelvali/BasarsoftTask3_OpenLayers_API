using BasarSoftTask3_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.ComponentModel;
using System.Reflection.Metadata;

namespace BasarSoftTask3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogAnalyseController : ControllerBase
    {
        private readonly ElasticClient _client;

        public LogAnalyseController()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                       .DefaultIndex("logs_v3").DefaultMappingFor<DTOs.Log>(m => m.IndexName("logs_v3")); // logs index name
            _client = new ElasticClient(settings);
        }

        [HttpGet("{logTime}")]
        public async Task<IActionResult> GetLogs(int logTime)
        {
            try
            {
                //var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                var now = DateTime.UtcNow;
                DateTime fromDate;
                // Daha kapsamlı zaman aralığı hesaplaması
                if (logTime == 1)
                {
                    fromDate = now.AddDays(-1); // Son 1 gün
                }
                else if (logTime == 7)
                {
                    fromDate = now.AddDays(-7); // Son 1 hafta
                }
                else if (logTime == 30)
                {
                    fromDate = now.AddDays(-30); // Son 1 ay
                }
                else
                {
                    return BadRequest("Invalid logTime value. It should be 1, 7, or 30.");
                }

                var searchResponse = _client.Search<Log>(s => s
                    .Size(1000)
                   .Query(q => q
                       .DateRange(r => r
                           .Field(f => f.Timestamp)
                           .GreaterThanOrEquals(DateMath.Anchored(fromDate))
                           .LessThanOrEquals(DateMath.Now)
                       )
                   )
               );

                if (searchResponse.IsValid)
                {
                    var hits = searchResponse.Hits;
                    var logs = hits.Select(hit => hit.Source);
                    return Ok(logs);
                }
                else
                {
                    // Elasticsearch sorgusu geçersizse, hatayı logla
                    Console.WriteLine($"Elasticsearch error: {searchResponse.DebugInformation}");
                    return StatusCode(500, "An error occurred while processing the request.");
                }
            }
            catch (Exception ex)
            {

                // Elasticsearch isteği sırasında bir istisna oluşursa, hatayı logla ve 500 hatası döndür
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }

        }
    }
}
