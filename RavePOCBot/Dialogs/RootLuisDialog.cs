namespace RavePOCBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Configuration;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Microsoft.Integration.Bot.Helpers;
    using RavePOCBot.Cards;
    using SharePointPOCLib;

    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        public RootLuisDialog(LuisModelAttribute luisModelAttribute):base(new LuisService(luisModelAttribute))
        {
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            context.SendTypingAcitivity();
            var k = new QnAMakerService(new QnAMakerAttribute(WebConfigurationManager.AppSettings["QNAAuthKey"], WebConfigurationManager.AppSettings["QNAKnowledgeBaseId"], "Sorry Could not get that", .75, endpointHostName: WebConfigurationManager.AppSettings["QNAEndpointUrl"]));
            await context.Forward(new QnADialog(k), this.ResumeAfter, context.Activity, CancellationToken.None);
        }

        private async Task ResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            await result;
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            context.SendTypingAcitivity();
            var feedback = ((Activity)context.Activity).CreateReply("Hi! Try asking me questions on Troubleshooting or select below options");
            feedback.SuggestedActions = ResultCard.GetSuggestedActions();
            await context.PostAsync(feedback);

            context.SendTypingAcitivity();
            var k = new QnAMakerService(new QnAMakerAttribute(WebConfigurationManager.AppSettings["QNAAuthKey"], WebConfigurationManager.AppSettings["QNAKnowledgeBaseId"], "Sorry Could not get that", .75, endpointHostName: WebConfigurationManager.AppSettings["QNAEndpointUrl"]));
            await context.Forward(new QnADialog(k), this.ResumeAfter, context.Activity, CancellationToken.None);

        }

    }
}
