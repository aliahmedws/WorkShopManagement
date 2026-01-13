using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkShopManagement.External.CarsXE
{
    public sealed class ImagesResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("query")]
        public ImagesQueryDto? Query { get; set; }

        [JsonPropertyName("images")]
        public List<ImageItemDto>? Images { get; set; }
    }

    public sealed class ImagesQueryDto
    {
        [JsonPropertyName("year")]
        public string? Year { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("format")]
        public string? Format { get; set; }

        [JsonPropertyName("angle")]
        public string? Angle { get; set; }

        // Captures any other query parameters sent back
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? Extra { get; set; }
    }

    public sealed class ImageItemDto
    {
        [JsonPropertyName("mime")]
        public string? Mime { get; set; }

        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("contextLink")]
        public string? ContextLink { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("byteSize")]
        public long? ByteSize { get; set; }

        [JsonPropertyName("thumbnailLink")]
        public string? ThumbnailLink { get; set; }

        [JsonPropertyName("thumbnailHeight")]
        public int? ThumbnailHeight { get; set; }

        [JsonPropertyName("thumbnailWidth")]
        public int? ThumbnailWidth { get; set; }

        [JsonPropertyName("hostPageDomainFriendlyName")]
        public string? HostPageDomainFriendlyName { get; set; }

        [JsonPropertyName("accentColor")]
        public string? AccentColor { get; set; }

        [JsonPropertyName("datePublished")]
        public string? DatePublished { get; set; }
    }
}

