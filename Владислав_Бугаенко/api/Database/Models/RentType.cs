using System.Text.Json.Serialization;

namespace api.Database.Models;

public partial class RentType
{
    public long Id { get; set; }

    public string RentType1 { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
