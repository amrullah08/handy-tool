using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RavePOCBot.Common
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class QnAResult
    {
        [JsonProperty("answers")]
        public Answer[] Answers { get; set; }
    }

    public partial class Answer
    {
        [JsonProperty("questions")]
        public string[] Questions { get; set; }

        [JsonProperty("answer")]
        public string AnswerAnswer { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("metadata")]
        public object[] Metadata { get; set; }
    }

    public partial class QnAResult
    {
        public static QnAResult FromJson(string json) => JsonConvert.DeserializeObject<QnAResult>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this QnAResult self) => JsonConvert.SerializeObject(self, Converter.Settings);
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