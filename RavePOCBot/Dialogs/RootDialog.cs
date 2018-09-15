using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Integration.Bot.Helpers;
using Newtonsoft.Json;
using QnAMaker;
using RavePOCBot.Cards;

namespace RavePOCBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<string>
    {
        Task IDialog<string>.StartAsync(IDialogContext context)
        {
            context.Wait(InitialUserQuery);
            return Task.FromResult(true);
        }
        public async Task InitialUserQuery(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var response = await argument;
            await context.SendTypingAcitivity();
            context.PrivateConversationData.SetValue("topic", response.Text);
            var rk = LuisFetcher.GetAnswers(response.Text.ToString()).Result;
            await context.SendTypingAcitivity();

            switch (rk.TopScoringIntent.IntentIntent)
            {
                case "outlook":
                    context.PrivateConversationData.SetValue("Intent", "Outlook");
                    context.PrivateConversationData.SetValue("IntentQuery", response.Text);
                    var re = context.MakeMessage();
                    re.Text = "I have solution for Outlook TOP call generators. Please select the relevant options from below";
                    var qnAResults = QnAMaker.QnAFetchter.GetAnswers("Get Outlook Bot Options").Result;
                    //re.SuggestedActions = new SuggestedActions()
                    //{
                    //    Actions = new List<CardAction>()
                    //};
                    await context.SendTypingAcitivity();
                    //re.SuggestedActions = ResultCard.GetSuggestedQnAActions((qnAResults.Answers[0].AnswerAnswer + "," + others).Split(','));
                    ResultCard resultCard = new ResultCard();
                    resultCard.CustomQnACard(re, qnAResults);
                    await context.PostAsync(re);
                    context.Wait(HandleTopOncallGenerators);
                    break;
                case "mailbox":
                    context.PrivateConversationData.SetValue("Intent", "mailbox");
                    context.PrivateConversationData.SetValue("IntentQuery", response.Text);
                    re = context.MakeMessage();
                    re.Text = "I have solution for mailbox TOP call generators. Please select the relevant options from below";
                    qnAResults = QnAMaker.QnAFetchter.GetAnswers("Get mailbox Bot Options").Result;
                    re.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                    };
                    await context.SendTypingAcitivity();
                    re.SuggestedActions = ResultCard.GetSuggestedQnAActions((qnAResults.Answers[0].AnswerAnswer + "," + others).Split(','));
                    await context.PostAsync(re);
                    context.Wait(HandleTopOncallGenerators);
                    break;
                default:break;
            }

        }

        string others = "Others solution is not listed";

        public async Task IssueResolved(IDialogContext context)
        {
            var re = context.MakeMessage();
            re.Text = "How was your experience with Rave Automated Bot (1-5)?";

            re.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
            };
            await context.SendTypingAcitivity();
            re.SuggestedActions.Actions = new List<CardAction>(new CardAction[] {
            new CardAction() { Title = "1", Type = ActionTypes.PostBack, Value = "1" },
            new CardAction() { Title = "2", Type = ActionTypes.PostBack, Value = "2" },
            new CardAction() { Title = "3", Type = ActionTypes.PostBack, Value = "3" },
            new CardAction() { Title = "4", Type = ActionTypes.PostBack, Value = "4" },
            new CardAction() { Title = "5", Type = ActionTypes.PostBack, Value = "5" }
        });

            await context.PostAsync(re);
            context.Wait(IssueResolvedWaitHandler);
        }

        public async Task IssueResolvedWaitHandler(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await context.PostAsync("Thank you for providing valuable feedback. Please let me know if you have any additional queries");
            context.Done("completed");

        }

            public async Task HandleTopOncallGenerators(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var response = await argument;
            await context.SendTypingAcitivity();

            if (!response.Text.Equals(others))
            {
                var k = QnAMaker.QnAFetchter.GetAnswers(response.Text).Result;
                await context.PostAsync(k.Answers[0].AnswerAnswer);
            }
            else
            {
                var k = QnAMaker.QnAFetchter.GetAnswers((context.PrivateConversationData.GetValue<string>("IntentQuery"))).Result;
                await context.PostAsync(k.Answers[0].AnswerAnswer);

            }
            ResultCard resultCard = new ResultCard();
            await resultCard.PostAsyncWithConvertToOptionsCard(context, "Did this help you?", issueSolvedCardAction);
            context.Wait(this.DoYouWantMoreQna);
        }

        static string issueResolved = "Yes, Issue is Resolved";
        static string doYouWantMore = "No, Issue is not resolved";

        static string[] issueSolvedCardAction = new string[] { issueResolved, doYouWantMore };

        private async Task CustomSearch(IDialogContext context, string text)
        {

            var subscriptionKey = System.Configuration.ConfigurationManager.AppSettings["BingCustomSearchKey"];
            var customConfigId = System.Configuration.ConfigurationManager.AppSettings["BingConfigId"];
            var searchTerm = text;

            var url = "https://api.cognitive.microsoft.com/bingcustomsearch/v7.0/search?" +
                "q=" + searchTerm +
                "&customconfig=" + customConfigId;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            var httpResponseMessage = client.GetAsync(url).Result;
            var responseContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
            BingCustomSearchResponse response = JsonConvert.DeserializeObject<BingCustomSearchResponse>(responseContent);

            ResultCard resultCard = new ResultCard();

            IMessageActivity msg = context.MakeMessage();
            resultCard.CustomCard(msg, response.webPages);
            await context.PostAsync(msg);
        }

        private async Task DoYouWantMoreQna(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            if (response.Text.Equals(issueResolved))
            {
                await this.IssueResolved(context);
                return;
            }

            await context.SendTypingAcitivity();

            await this.CustomSearch(context, context.PrivateConversationData.GetValue<string>("IntentQuery"));
            
            ResultCard resultCard = new ResultCard();
            await resultCard.PostAsyncWithConvertToOptionsCard(context, "We have suggested possible options", issueSolvedCardAction);

            context.Wait(this.Completed);

        }

        private async Task Completed(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;
            await context.SendTypingAcitivity();

            if (response.Text.Equals(issueResolved))
            {
                await this.IssueResolved(context);
                return;
            }


            await context.PostAsync("PLEASE COLLECT THE RELEVANT Information (OFFCAT LOGS, ETL Logs. etc. ) for '" + context.PrivateConversationData.GetValue<string>("IntentQuery") + "' AND REACHOUT TO YOUR NEXT TEAM FOR FURTHER ASSISTANCE");
            context.Done("");
        }
    }
}