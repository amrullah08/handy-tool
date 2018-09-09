// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var LuisResult = LuisResult.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class LuisResult
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("topScoringIntent")]
        public Intent TopScoringIntent { get; set; }

        [JsonProperty("intents")]
        public Intent[] Intents { get; set; }

        [JsonProperty("entities")]
        public object[] Entities { get; set; }

        [JsonProperty("sentimentAnalysis")]
        public SentimentAnalysis SentimentAnalysis { get; set; }
    }

    public partial class Intent
    {
        [JsonProperty("intent")]
        public string IntentIntent { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }

    public partial class SentimentAnalysis
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }

    public partial class LuisResult
    {
        public static LuisResult FromJson(string json) => JsonConvert.DeserializeObject<LuisResult>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this LuisResult self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
