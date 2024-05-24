using Nest;

namespace BasarSoftTask3_API.DTOs
{

    public class Log
    {
        //    public LogMessage Message { get; set; }
        //}

        //public class LogMessage
        //{
        //    public string Method { get; set; }
        //    public PathInfo Path { get; set; }
        //    public string Role { get; set; }
        //    public string UserName { get; set; }
        //    public string Body { get; set; }
        //    public RouteData RouteData { get; set; }
        //    public DateTime Timestamp { get; set; }
        //}

        //public class PathInfo
        //{
        //    public string Value { get; set; }
        //    public bool HasValue { get; set; }
        //}

        //public class RouteData
        //{
        //    public string Action { get; set; }
        //    public string Controller { get; set; }
        //}
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
