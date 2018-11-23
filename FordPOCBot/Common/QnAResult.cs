namespace RavePOCBot.Common
{
    using Newtonsoft.Json;

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
        public static QnAResult FromJson(string json) => JsonConvert.DeserializeObject<QnAResult>(json, RavePOCBot.Common.Serialize.Settings);
    }
}