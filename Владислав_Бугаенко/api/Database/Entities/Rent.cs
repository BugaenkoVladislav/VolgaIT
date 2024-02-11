
using System.Text.Json.Serialization;

namespace api.Database.Models;

public partial class Rent
{
    public long Id { get; set; }

    public DateTime TimeStart { get; set; }

    public DateTime? TimeEnd { get; set; }

    public long IdTransport { get; set; }

    public long IdUser { get; set; }

    public double PriceOfUnit { get; set; }

    public long PriceType { get; set; }

    public double? FinalPrice { get; set; }

    [JsonIgnore]
    public virtual Transport IdTransportNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual User IdUserNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual RentType PriceTypeNavigation { get; set; } = null!;
}
