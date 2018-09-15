using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using RavePOCBot.Cards;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RavePOCBot.Dialogs
{
    public class OutLookDialog : IDialog<string>
    {
        Task IDialog<string>.StartAsync(IDialogContext context)
        {
            context.Wait(SuggestedActions);
            return Task.FromResult(true);
        }

        public async Task SuggestedActions(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var response = await argument;
            context.PrivateConversationData.SetValue("topic", response.Text);
            var k = QnAMaker.QnAFetchter.GetAnswers(response.Text).Result;

            await context.PostAsync(k.Answers[0].AnswerAnswer);

            var re = context.MakeMessage();
            re.Text = "Did this help you?";
            re.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
            };
            //foreach (var r in result)
            {
                re.SuggestedActions.Actions.Add(new CardAction() { Title = "Do you want more", Type = ActionTypes.PostBack, Value = $"Do you want more" });
            }

            await context.PostAsync(re);
            context.Wait(this.DoYouWantMore);
        }

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

        private async Task DoYouWantMore(IDialogContext context, IAwaitable<object> result)
        {
            var respoonse = await result;
            await context.PostAsync("Enter your query");
            context.Wait(QnAHandler);
        }

        private async Task QnAHandler(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var respoonse = await result;
            var k = QnAMaker.QnAFetchter.GetAnswers(respoonse.Text).Result;
            // await context.PostAsync(k.Answers[0].AnswerAnswer);
            var re = context.MakeMessage();
            re.Text = k.Answers[0].AnswerAnswer;
            re.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
            };
            //foreach (var r in result)
            {
                re.SuggestedActions.Actions.Add(new CardAction() { Title = "Do you want more", Type = ActionTypes.PostBack, Value = $"{respoonse.Text}" });
            }

            await context.PostAsync(re);
            context.Wait(this.DoYouWantMoreQna);
        }

        private async Task DoYouWantMoreQna(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var respoonse = await result;
            this.CustomSearch(context, respoonse.Text);
            var re = context.MakeMessage();
            re.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
            };
            re.Text = "We have suggested possible options";
            //foreach (var r in result)
            {
                re.SuggestedActions.Actions.Add(new CardAction() { Title = "Do you want more", Type = ActionTypes.PostBack, Value = $"Do you want more" });
            }

            await context.PostAsync(re);
            context.Wait(this.Completed);
        }

        private async Task Completed(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync("COLLECT OFFCAT LOGS, ETL Logs and reach out to your NEXT TEAM agent");
            context.Done("");
        }
    }
}