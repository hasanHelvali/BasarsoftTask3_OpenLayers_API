using NetTopologySuite.Geometries;

namespace BasarSoftTask3_API.DTOs
{
    public class Feature
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Dictionary<string, object> properties { get; set; }
    }
}
