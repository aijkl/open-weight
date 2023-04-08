using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aijkl.OpenWeight.Database.Entities; 
public class WeightEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public double Weight { get; set; }
    public DateTime Timestamp { set; get; }
}