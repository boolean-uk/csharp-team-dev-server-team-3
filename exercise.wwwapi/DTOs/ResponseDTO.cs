using System.Text.Json.Serialization;

namespace exercise.wwwapi.DTOs
{
    public class ResponseDTO<T>
    {
        [JsonPropertyName("message")]
        public required string Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        public ResponseDTO() { Timestamp = DateTime.UtcNow; }

        // For convenience
        //public ResponseDTO(string message, T? inputObject = default)
        //{
        //    Message = message;
        //    Data = inputObject;
        //    Timestamp = DateTime.UtcNow;
        //}
    }
}