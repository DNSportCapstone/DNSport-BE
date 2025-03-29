using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DataAccess.DTOs.Response
{
    public class DistanceMatrixResponse
    {
        [JsonPropertyName("rows")]
        public List<Row>? Rows { get; set; }
    }

    public class Row
    {
        [JsonPropertyName("elements")]
        public List<Element>? Elements { get; set; }
    }

    public class Element
    {
        [JsonPropertyName("distance")]
        public DistanceInfo? Distance { get; set; }

        [JsonPropertyName("duration")]
        public DurationInfo? Duration { get; set; }
    }

    public class DistanceInfo
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class DurationInfo
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

}
