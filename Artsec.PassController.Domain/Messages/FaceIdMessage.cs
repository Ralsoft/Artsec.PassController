using System.Text.Json.Serialization;

namespace Artsec.PassController.Domain.Messages;

public class FaceIdMessage
{
    [JsonPropertyName("cam_id")] public string CamId { get; set; } = string.Empty;
    [JsonPropertyName("id")] public string FaceId { get; set; } = string.Empty;
    [JsonPropertyName("src_action")] public string SrcAction { get; set; } = string.Empty;
    [JsonPropertyName("time")] public DateTime Time { get; set; }
}
