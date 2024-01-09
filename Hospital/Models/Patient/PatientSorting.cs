using System.Text.Json.Serialization;

namespace Hospital.Models.Patient
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PatientSorting
    {
        NameAsc,
        NameDesc,
        CreateAsc,
        CreateDesc,
        InspectionAsc,
        InspectionDesc
    }
}
