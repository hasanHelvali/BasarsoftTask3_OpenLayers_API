using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BasarSoftTask2_API.Entities
{
    public class LocAndUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Geometry Geometry { get; set; }
    }
}
