using BasarSoftTask3_API.Logging;
using BasarSoftTask3_API.Services;
using System.Linq;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using BasarSoftTask3_API.DTOs;

namespace BasarSoftTask3_API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LogProducer _logProducer;

        public LoggingMiddleware(RequestDelegate next, LogProducer logProducer)
        {
            _next = next;
            _logProducer = logProducer;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering(); //isteğin gövdesinin bir kez daha okunabilmesini sağlar.
            var body="";
            // İsteği logla
            var request = context.Request;
            //string bearerToken = request.Headers["Authorization"].FirstOrDefault();
            if (request.Method.ToString() != "OPTIONS")
            {
                string? bearerToken= request.Headers["Authorization"].ToString().Replace("Bearer ", "").TrimStart().TrimEnd();
                string role = TokenHandler.GetRole(bearerToken);
                string userName = TokenHandler.GetUserName(bearerToken);
                
                if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put || context.Request.Method == HttpMethods.Delete)
                {
                    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                    {
                        body = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0; // Gövdeyi yeniden okumak için konumu başa sar
                    }
                }
                var log = new
                {
                    Method = request.Method,
                    Path = request.Path,
                    Role=role,
                    userName=userName,
                    body= body,
                    //queryString= request.QueryString,
                    routeData = new 
                    {
                        Action = request.RouteValues["action"].ToString(),
                        Controller = request.RouteValues["controller"].ToString()
                    },
                    Timestamp = DateTime.UtcNow
                };
                //var logMessage = JsonSerializer.Serialize(log);
                var logMessage = System.Text.Json.JsonSerializer.Serialize(log, new JsonSerializerOptions
                {
                    WriteIndented = true // Daha okunabilir olması için
                });
                _logProducer.SendLog(logMessage);
            }  
            // Sonraki middleware'e geç
            await _next(context);
        }
    }
}
