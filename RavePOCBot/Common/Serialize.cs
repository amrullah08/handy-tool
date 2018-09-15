using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace RavePOCBot.Common
{
    public static class Serialize
    {
        public static string ToJson(this LuisResult self) => JsonConvert.SerializeObject(self, Settings);

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