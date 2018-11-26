using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.PersonalityChat.Core;
using Microsoft.Bot.Connector;
using Microsoft.Integration.Bot.Helpers;
using Newtonsoft.Json;
using QnAMaker;
using FordPOCBot.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FordPOCBot.Common;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Integration.Bot.Cards;

namespace FordPOCBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<string>
    {
        Task IDialog<string>.StartAsync(IDialogContext context)
        {
            context.Wait(InitializeFord);
            return Task.FromResult(true);
        }

        public async Task InitializeFord(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var response = await argument;
            var rk = LuisFetcher.GetAnswers(response.Text.ToString()).Result;
            await context.SendTypingAcitivity();

            switch (rk.TopScoringIntent.IntentIntent)
            {
                case "FordSiteInfo":
                    var answer = QnAMaker.QnAFetchter.GetAnswers("whats this site?").
            Result.Answers[0].AnswerAnswer;
                    MediaUrl mediaUrl = new MediaUrl("https://marczak.io/images/botseries-rich-cards/CreatingBot.mp4");
                    await context.PostAsync(answer);
                    var k = ResultCard.GetVideoCard("Welcome to Leadership Learning Site", "", "Ford HR ", mediaUrl);
                    var reply = context.MakeMessage();
                    reply.Attachments = new List<Attachment>();
                    reply.Attachments.Add(k);
                    await context.PostAsync(reply);
                    context.Done("competed");
                    break;
                case "FordCurrentAffair":
                    answer = QnAMaker.QnAFetchter.GetAnswers("Whats cooking?").
            Result.Answers[0].AnswerAnswer;
                    await context.PostAsync(answer);
                    ResultCard result = new ResultCard();
                    reply = context.MakeMessage();
                    reply.Text = "Would you like to learn more?";
                    reply.SuggestedActions = ResultCard.GetSuggestedQnAActions(new[] { "yes", "no" });
                    await context.PostAsync(reply);
                    context.Wait(handleSiteInfo);
                    break;
                case "FordModels":
                    ResultCard resultCard = new ResultCard();
                    reply = context.MakeMessage();
                    resultCard.ConvertToOptionsCard(reply, new[] { "Critical Thinking", "Design Thinking", "System Thinking" });
                    await context.PostAsync(reply);
                    context.Wait(FordModelSelection);
                    break;
                case "FordTraining":
                    result = new ResultCard();
                    reply = context.MakeMessage();
                    reply.Text = "If training was to be made available what would be your preference be:";
                    reply.SuggestedActions = ResultCard.GetSuggestedQnAActions(new[] { "Webex Training", "Class Room" });
                    await context.PostAsync(reply);
                    context.Wait(handleLocationTraining);
                    break;
                default:break;
            }
        }

        private async Task handleLocationTraining(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;
            if (response.Text.Equals("Webex Training"))
            {
                var reply = context.MakeMessage();
                reply.Text = "Got it, which time would you prefer?";
                reply.SuggestedActions = ResultCard.GetSuggestedQnAActions(new[] { "Morning", "Evening" });
                await context.PostAsync(reply);
                context.Wait(CompleteTraining);
            }
        }

        private async Task CompleteTraining(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;
            if (response.Text.Equals("Morning"))
            {
                ResultCard resultCard = new ResultCard();
                var reply = context.MakeMessage();
                reply.Text = FordResources.TrainingRequest;
                await context.PostAsync(reply);
                context.Wait(DisplayFeedback);
            }
        }

        private async Task FordModelSelection(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;
            if (response.Text.Equals("Design Thinking"))
            {
                ResultCard resultCard = new ResultCard();
                var reply = context.MakeMessage();
                reply.Text = "Okay thanks, how would you like to complete this learning:";
                reply.SuggestedActions = ResultCard.GetSuggestedQnAActions(new[] { "Bite Size Content", "Online Training Program" });
                await context.PostAsync(reply);
                context.Wait(HandleModelTrainingComplete);
            }
        }

        private async Task HandleModelTrainingComplete(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            var response = await result;
            if (response.Text.Equals("Online Training Program"))
            {
                await context.PostAsync("Great, click here to enrol into a self paced online training program on Design Thinking");
            }
            else if (response.Text.Equals("Bite Size Content"))
            {
                await context.PostAsync("Great, click here to view Bite Size learning content on “Design Thinking");
            }
            context.Done("Done Training");
        }

        private async Task handleSiteInfo(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;
            if (response.Text.Equals("no"))
            {
                await context.PostAsync("Is there anything else I can help you with?");
                context.Done("completed");
            }
            else
            {

                var answer = QnAMaker.QnAFetchter.GetAnswers("Do The Right Thing").
        Result.Answers[0].AnswerAnswer;
                await context.PostAsync(answer);

                var reply = context.MakeMessage();
                reply.Text = "We hope you have enjoyed the learning?";
                reply.SuggestedActions = ResultCard.GetSuggestedQnAActions(new[] { "yes", "no" });
                await context.PostAsync(reply);
                context.Wait(DisplayFeedback);
            }
        }

        private async Task DisplayFeedback(IDialogContext context, IAwaitable<object> result)
        {
            ResultCard resultCard = new ResultCard();
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(resultCard.FeedBack());
            await context.PostAsync(reply);
            context.Wait(HandleFeedback);
        }

        private async Task HandleFeedback(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Thank you for valuable feedback. we will work on improving experience");
            context.Done("Completed Conversation");
        }

        private async Task DisplayFeedback(IDialogContext context)
        {
            ResultCard resultCard = new ResultCard();
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(resultCard.FeedBack());
            await context.PostAsync(reply);
            context.Done("Completed Conversation");
        }

        private async Task DateSelected(IDialogContext context, IAwaitable<object> result)
        {
            await result;
        }

        private Task formComplete(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
    }
}