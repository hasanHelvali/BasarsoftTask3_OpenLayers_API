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
                       .DefaultIndex("logs_v2").DefaultMappingFor<DTOs.Log>(m => m.IndexName("logs_v2")); // logs index name
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

                //var searchResponse = await _client.SearchAsync<Log>(s => s.Index("logs_v2").From(0).Size(1000)
                //    .Query(q=>q.Bool(b=>b.Filter(f=>f.DateRange(dr=>dr.Field(f=>f.Message.Timestamp)
                //                        .GreaterThanOrEquals(fromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")) // Başlangıç tarihini belirtin
                //    .LessThanOrEquals(now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")))))));
                var searchResponse = await _client.SearchAsync<Log>(s => s.Index("logs_v2").From(0).Size(1000)
                .Query(q => q.Bool(b => b.Filter(f => f.DateRange(dr => dr.Field(f => f.Message.Timestamp)
                                    .GreaterThanOrEquals(fromDate) // Başlangıç tarihini belirtin
                .LessThanOrEquals(now))))));

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
