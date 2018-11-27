namespace FordPOCBot.Dialogs
{
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Microsoft.Integration.Bot.Helpers;
    using Newtonsoft.Json;
    using FordPOCBot.Cards;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Configuration;

    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        public RootLuisDialog(LuisModelAttribute luisModelAttribute) : base(new LuisService(luisModelAttribute))
        {
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            context.SendTypingAcitivity();

            var k = new QnAMakerService(new QnAMakerAttribute(WebConfigurationManager.AppSettings["QNAAuthKey"], WebConfigurationManager.AppSettings["QNAKnowledgeBaseId"], "Sorry Could not get that", .3, endpointHostName: WebConfigurationManager.AppSettings["QNAEndpointUrl"]));
            await context.Forward(new QnADialog(k), this.ResumeAfter, context.Activity, CancellationToken.None);
        }

        private async Task ResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            await result;
            context.Wait(this.MessageReceived);
        }
    }

    public class BingCustomSearchResponse
    {
        public string _type { get; set; }
        public WebPages webPages { get; set; }
    }

    public class WebPages
    {
        public string webSearchUrl { get; set; }
        public int totalEstimatedMatches { get; set; }
        public WebPage[] value { get; set; }
    }

    public class WebPage
    {
        public string name { get; set; }
        public string url { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public DateTime dateLastCrawled { get; set; }
        public string cachedPageUrl { get; set; }
        public OpenGraphImage openGraphImage { get; set; }
    }

    public class OpenGraphImage
    {
        public string contentUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
}