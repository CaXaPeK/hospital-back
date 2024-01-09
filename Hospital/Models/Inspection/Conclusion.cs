using System.Text.Json.Serialization;

namespace Hospital.Models.Inspection
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Conclusion
    {
        Disease,
        Recovery,
        Death
    }
}
