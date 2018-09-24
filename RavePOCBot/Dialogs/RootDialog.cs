using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.PersonalityChat.Core;
using Microsoft.Bot.Connector;
using Microsoft.Integration.Bot.Helpers;
using Newtonsoft.Json;
using QnAMaker;
using RavePOCBot.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RavePOCBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<string>
    {
        static string[] topOnCallTopics = QnAMaker.QnAFetchter.GetAnswers("Bot Custom Topics").Result.Answers[0].AnswerAnswer.Split(',');
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
            var selectedIntent = topOnCallTopics.FirstOrDefault(cc => cc.Equals(rk.TopScoringIntent.IntentIntent));

            if(selectedIntent != null)
            {
                context.PrivateConversationData.SetValue("Intent", selectedIntent);
                context.PrivateConversationData.SetValue("IntentQuery", response.Text);
                var re = context.MakeMessage();
                re.Text = "I have solution for " + selectedIntent + " TOP call generators. Please select the relevant options from below";
                var qnAResults = QnAMaker.QnAFetchter.GetAnswers("Get " + selectedIntent + " Bot Options").Result;
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
            }
            else if (selectedIntent == null && rk.TopScoringIntent.IntentIntent.Equals("greeting"))
            {
                PersonalityChatOptions personalityChatOptions = new PersonalityChatOptions(string.Empty, PersonalityChatPersona.Professional);
                PersonalityChatService personalityChatService = new PersonalityChatService(personalityChatOptions);

                var PersonalityChatResults = Task.FromResult<PersonalityChatResults>(await personalityChatService.QueryServiceAsync(rk.Query));
                string botOutput = PersonalityChatResults?.Result.ScenarioList?.FirstOrDefault()?.Responses?.FirstOrDefault() ?? "";

                if (!string.IsNullOrEmpty(botOutput))
                {
                    await context.PostAsync(botOutput);
                }

                var reply = context.MakeMessage();
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(ResultCard.ShowGreetingCard());
                var k = QnAMaker.QnAFetchter.GetAnswers("Get Bot Options").Result;
                await context.PostAsync(reply);
            }
            else
            {
                response.Text = others;
                await this.HandleTopOncallGenerators(context, argument);
            }

            //switch (rk.TopScoringIntent.IntentIntent)
            //{
            //    case "outlook":
            //        context.PrivateConversationData.SetValue("Intent", "Outlook");
            //        context.PrivateConversationData.SetValue("IntentQuery", response.Text);
            //        var re = context.MakeMessage();
            //        re.Text = "I have solution for Outlook TOP call generators. Please select the relevant options from below";
            //        var qnAResults = QnAMaker.QnAFetchter.GetAnswers("Get Outlook Bot Options").Result;
            //        //re.SuggestedActions = new SuggestedActions()
            //        //{
            //        //    Actions = new List<CardAction>()
            //        //};
            //        await context.SendTypingAcitivity();
            //        //re.SuggestedActions = ResultCard.GetSuggestedQnAActions((qnAResults.Answers[0].AnswerAnswer + "," + others).Split(','));
            //        ResultCard resultCard = new ResultCard();
            //        resultCard.CustomQnACard(re, qnAResults);
            //        await context.PostAsync(re);
            //        context.Wait(HandleTopOncallGenerators);
            //        break;

            //    case "mailbox":
            //        context.PrivateConversationData.SetValue("Intent", "mailbox");
            //        context.PrivateConversationData.SetValue("IntentQuery", response.Text);
            //        re = context.MakeMessage();
            //        re.Text = "I have solution for mailbox TOP call generators. Please select the relevant options from below";
            //        qnAResults = QnAMaker.QnAFetchter.GetAnswers("Get mailbox Bot Options").Result;
            //        re.SuggestedActions = new SuggestedActions()
            //        {
            //            Actions = new List<CardAction>()
            //        };
            //        await context.SendTypingAcitivity();
            //        re.SuggestedActions = ResultCard.GetSuggestedQnAActions((qnAResults.Answers[0].AnswerAnswer + "," + others).Split(','));
            //        await context.PostAsync(re);
            //        context.Wait(HandleTopOncallGenerators);
            //        break;

            //    default:
            //        response.Text = others;
            //        await this.HandleTopOncallGenerators(context, argument);
            //        break;
            //}
        }

        private string others = "Others solution is not listed";

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

        private static string issueResolved = "Yes, Issue is Resolved";
        private static string doYouWantMore = "No, Issue is not resolved";

        private static string[] issueSolvedCardAction = new string[] { issueResolved, doYouWantMore };

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

            var qnAResults = QnAMaker.QnAFetchter.GetAnswers("get " + context.PrivateConversationData.GetValue<string>("Intent") + " last message").Result;

            await context.PostAsync(qnAResults.Answers[0].AnswerAnswer);
            context.Done("");
        }
    }
}