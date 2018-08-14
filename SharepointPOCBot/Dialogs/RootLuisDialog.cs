namespace SharePointPOCBot.Dialogs
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
    using SharePointPOCBot.Cards;
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
            string message = $"Sorry, I did not understand {result.Query}. Here are few options.";
            IMessageActivity msg = context.MakeMessage();
            msg.Text = message;
            msg.SuggestedActions = ResultCard.GetSuggestedActions();
            await context.PostAsync(msg);
        }

        
        [LuisIntent("QNABase")]
        public async Task DownloadKnowledgeBase(IDialogContext context, LuisResult result)
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
            var feedback = ((Activity)context.Activity).CreateReply("Hi! Try asking me questions on Litigation cases, Legal or Internal investigation or select below options");
            feedback.SuggestedActions = ResultCard.GetSuggestedActions();
            await context.PostAsync(feedback);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Legal")]
        public async Task Legal(IDialogContext context, LuisResult result)
        {
            context.SendTypingAcitivity();
            LegalDocuments.RetrieveLegalDocuments();
            context.SendTypingAcitivity();
            var itms = LegalDocuments.Legaldocuments;
            var res = "There are " + itms.Where(cc => cc.OnPreservationHold).Count() + " documents are on preservation hold";

            var attch = ResultCard.GetThumbnailCard(
                    "Legal Cases",
                    res,
                    "Click below link to go the Sharepoint portal for Legal Cases",
                    new CardImage(url: "http://technoinfotech.com/images/legal-hold-technoarchive.png"),
                    new CardAction(ActionTypes.OpenUrl, "Learn More", value: "https://m365x844754.sharepoint.com/sites/ChrevronBot/Shared%20Documents/Forms/AllItems.aspx"));

            IMessageActivity msg = context.MakeMessage();
            msg.Attachments.Add(attch);
            await context.PostAsync(msg);



            msg = context.MakeMessage();
            ResultCard card = new ResultCard();
            card.RenderLegalDocuments(msg, LegalDocuments.Legaldocuments);
            await context.PostAsync(msg);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("InternalInvestigation")]
        public async Task InternalInvestigation(IDialogContext context, LuisResult result)
        {
            EntityRecommendation obj = null;
            result.TryFindEntity("builtin.number", out obj);
            context.SendTypingAcitivity();
            InternalInvestigationDocument.RetrieveInvestigation();
            context.SendTypingAcitivity();
            var itms = InternalInvestigationDocument.InternalInvestigationDocuments;
            var res = "There are " + itms.Where(cc => cc.YearOfInternalInvestigation == Convert.ToInt32(obj.Entity)).Count() + " substantiated internal investigation cases";

            await context.PostAsync(res);

            var attch = ResultCard.GetThumbnailCard(
                    "Internal Investigation Cases",
                    res,
                    "Click below link to go the Sharepoint portal for Internal Investigation Case",
                    new CardImage(url: "http://compliancestrategists.com/csblog/wp-content/uploads/2016/10/Investigation-300x195.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "Learn more", value: " https://m365x844754.sharepoint.com/sites/ChrevronBot/Lists/Internal%20Investigation%20Case%20Tracking/AllItems.aspx"));


            IMessageActivity msg = context.MakeMessage();
            msg.Attachments.Add(attch);
            await context.PostAsync(msg);

            msg = context.MakeMessage();
            ResultCard card = new ResultCard();
            card.RenderInternalDocuments(msg, itms);
            await context.PostAsync(msg);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Litigation")]
        public async Task Litigation(IDialogContext context, LuisResult result)
        {
            EntityRecommendation LitigationFromDate, LitigationToDate;
            var res = result.Entities[0].Entity.ToLower()
                .Replace(" ", "").Replace("between", "").Split(new string[] { "to" }, StringSplitOptions.RemoveEmptyEntries);

            DateTime fromDate = Convert.ToDateTime(res[0]), toDate = Convert.ToDateTime(res[1]);

            context.SendTypingAcitivity();
            LitigationDocument.RetrieveLitigation();
            context.SendTypingAcitivity();
            var itms = LitigationDocument.LitigationDocuments;
            var results = itms.Where(cc => cc.ActiveCase && (cc.DateTracking >= fromDate && cc.DateTracking <= toDate));

            var dt = "There are " + results.Count() + " active litigation cases";

            var attch = ResultCard.GetThumbnailCard(
                    "Litigation cases",
                    dt,
                    "Click below link to go the Sharepoint portal for Litigation Case",
                    new CardImage(url: "http://www.ipwatchdog.com/wp-content/uploads/2016/01/litigation-red-335.jpg"),
                    new CardAction(ActionTypes.OpenUrl, "Learn more", value: "https://m365x844754.sharepoint.com/sites/ChrevronBot/LitigationCases/Forms/AllItems.aspx"));


            IMessageActivity msg = context.MakeMessage();
            msg.Attachments.Add(attch);
            await context.PostAsync(msg);


            msg = context.MakeMessage();
            ResultCard card = new ResultCard();
            card.RenderLitigationDocuments(msg, itms);
            await context.PostAsync(msg);

            context.Wait(this.MessageReceived);
        }
    }
}
