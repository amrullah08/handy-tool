using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RavePOCBot.Common
{
    public class StateConstants
    {

        public static string IntentQuery { get; set; } = "IntentQuery";
        public static string Intent { get; set; } = "Intent";

        public static string Topic { get; set; } = "Topic";

        public static string TopicSynonym { get; set; } = "TopicSynonym";
        public static string TopicSynonymInteractiveQuestion { get; set; } = "TopicSynonymInteractiveQuestion";
        public static string QuestionairreSeparator { get; set; } = "##";
    }
}