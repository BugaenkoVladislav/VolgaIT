using System.Text.Json.Serialization;

namespace api.Database.Models;

public partial class Color
{
    public long Id { get; set; }

    public string Color1 { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
}
