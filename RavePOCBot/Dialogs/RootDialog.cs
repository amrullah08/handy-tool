using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
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
            context.PrivateConversationData.SetValue("topic", response.Text);
            var rk = LuisFetcher.GetAnswers(response.Text.ToString()).Result;

            switch (rk.TopScoringIntent.IntentIntent)
            {
                case "outlook":
                    context.PrivateConversationData.SetValue("Intent", "Outlook");
                    context.PrivateConversationData.SetValue("IntentQuery", response.Text);
                    var re = context.MakeMessage();
                    re.Text = "Below are the top outlook oncall generators , please select below?";
                    var qnAResults = QnAMaker.QnAFetchter.GetAnswers("Get Outlook Bot Options").Result;
                    re.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                    };
                    re.SuggestedActions = ResultCard.GetSuggestedQnAActions((qnAResults.Answers[0].AnswerAnswer + "," + others).Split(','));
                    await context.PostAsync(re);
                    context.Wait(HandleTopOncallGenerators);
                    break;
                default:break;
            }

        }

        string others = "Others solution is not listed";

        public void IssueResolved(IDialogContext context)
        {
            context.PostAsync("my pleasure assisting you");
            context.Done("completed");
        }

        public async Task HandleTopOncallGenerators(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var response = await argument;

            if (!response.Text.Equals(others))
            {
                var k = QnAMaker.QnAFetchter.GetAnswers(response.Text).Result;
                await context.PostAsync(k.Answers[0].AnswerAnswer);
                this.IssueResolved(context);
                return;
            }
            else
            {
                var k = QnAMaker.QnAFetchter.GetAnswers((context.PrivateConversationData.GetValue<string>("IntentQuery"))).Result;
                await context.PostAsync(k.Answers[0].AnswerAnswer);

                var re = context.MakeMessage();
                re.Text = "Did this help you?";
                re.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                };
                //foreach (var r in result)
                {
                    re.SuggestedActions.Actions.Add(new CardAction() { Title = doYouWantMore, Type = ActionTypes.PostBack, Value = doYouWantMore });
                    re.SuggestedActions.Actions.Add(new CardAction() { Title = issueResolved, Type = ActionTypes.PostBack, Value = issueResolved });
                }

                await context.PostAsync(re);
                context.Wait(this.DoYouWantMoreQna);
            }
        }

        string issueResolved = "Issue is Resolved";
        string doYouWantMore = "Do you want more";

        private void CustomSearch(IDialogContext context, string text)
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
            context.PostAsync(msg);
        }

        private async Task DoYouWantMoreQna(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            if (response.Text.Equals(issueResolved))
            {
                this.IssueResolved(context);
                return;
            }


            this.CustomSearch(context, context.PrivateConversationData.GetValue<string>("IntentQuery"));
            var re = context.MakeMessage();
            re.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
            };
            re.Text = "We have suggested possible options";
            //foreach (var r in result)

            //foreach (var r in result)
            {
                re.SuggestedActions.Actions.Add(new CardAction() { Title = doYouWantMore, Type = ActionTypes.PostBack, Value = doYouWantMore });
                re.SuggestedActions.Actions.Add(new CardAction() { Title = issueResolved, Type = ActionTypes.PostBack, Value = issueResolved });
            }

            await context.PostAsync(re);
            context.Wait(this.Completed);

        }

        private async Task Completed(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            if (response.Text.Equals(issueResolved))
            {
                this.IssueResolved(context);
                return;
            }


            await context.PostAsync("PLEASE COLLECT THE RELEVANT Information (OFFCAT LOGS, ETL Logs.. ) for '" + context.PrivateConversationData.GetValue<string>("IntentQuery") + "' AND REACHOUT TO YOUR NEXT TEAM FOR FURTHER ASSISTANCE");
            context.Done("");
        }
    }
}