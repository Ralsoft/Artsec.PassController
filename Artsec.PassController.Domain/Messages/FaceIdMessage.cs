using System.Text.Json.Serialization;

namespace Artsec.PassController.Domain.Messages;

public class FaceIdMessage
{
    [JsonPropertyName("cam_id")] public int CamId { get; set; }
    [JsonPropertyName("id")] public string FaceId { get; set; } = string.Empty;
    [JsonPropertyName("src_action")] string SrcAction { get; set; } = string.Empty;
    [JsonPropertyName("time")] DateTime Time { get; set; }
}
