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
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";


            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        
        [LuisIntent("QNABase")]
        public async Task DownloadKnowledgeBase(IDialogContext context, LuisResult result)
        {
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
            var feedback = ((Activity)context.Activity).CreateReply("Hi! Try asking me things like 'Litigation cases', 'Legal' or 'Internal investigation'");
            feedback.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title = "preservation hold", Type=ActionTypes.PostBack, Value=$"1.)	How many documents are on preservation hold for this SP Online site?" },
                    new CardAction(){ Title = "active litigation cases", Type=ActionTypes.PostBack, Value=$"2.)	How many active litigation cases did we receive between 1/1/2017 to 8/12/2018?" },
                    new CardAction(){ Title = "internal investigation cases", Type=ActionTypes.PostBack, Value=$"3.)	How many substantiated internal investigation cases do we have in 2018?" }
                }
            };
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
            await context.PostAsync("There are " + itms.Where(cc => cc.OnPreservationHold).Count() + " documents are on preservation hold for this SP Online site");
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
            await context.PostAsync("There are " + itms.Where(cc => cc.YearOfInternalInvestigation == Convert.ToInt32(obj.Entity)).Count() + " substantiated internal investigation cases");
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
            await context.PostAsync("There are " + results.Count() + " substantiated internal investigation cases");

            context.Wait(this.MessageReceived);
        }
    }
}
