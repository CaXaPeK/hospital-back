using System.Text.Json.Serialization;

namespace Hospital.Models.General
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,
        Female
    }
}
