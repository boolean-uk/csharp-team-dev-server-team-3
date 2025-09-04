using System.Text.Json.Serialization;

namespace exercise.wwwapi.DTOs
{
    public class ResponseDTO<T>
    {
        [JsonPropertyName("status")]
        public required string Status { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }
    }
}
