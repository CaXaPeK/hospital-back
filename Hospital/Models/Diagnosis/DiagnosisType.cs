using System.Text.Json.Serialization;

namespace Hospital.Models.Diagnosis
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiagnosisType
    {
        Main,
        Concomitant,
        Complication
    }
}
