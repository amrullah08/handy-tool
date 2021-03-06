﻿using Microsoft.Bot.Builder.Dialogs;
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
using RavePOCBot.Common;

namespace RavePOCBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<string>
    {
        static string[] topOnCallTopics = QnAMaker.QnAFetchter.GetAnswers("Bot Custom Topics").
            Result.Answers[0].AnswerAnswer.Trim().Replace(" , ",",").Replace(" ,",",").Replace(", ",",").Split(',');

        Task IDialog<string>.StartAsync(IDialogContext context)
        {
            context.Wait(InitialUserQuery);
            return Task.FromResult(true);
        }

        public async Task InitialUserQuery(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var response = await argument;
            await context.SendTypingAcitivity();
            context.PrivateConversationData.SetValue(StateConstants.Topic, response.Text);
            var rk = LuisFetcher.GetAnswers(response.Text.ToString()).Result;
            await context.SendTypingAcitivity();
            var selectedIntent = topOnCallTopics.FirstOrDefault(cc => cc.Equals(rk.TopScoringIntent.IntentIntent) && rk.TopScoringIntent.Score > Constants.MaxLUIScore);

            context.PrivateConversationData.SetValue(StateConstants.IntentQuery, response.Text);

            if (selectedIntent != null)
            {
                context.PrivateConversationData.SetValue(StateConstants.Intent, selectedIntent);
                var re = context.MakeMessage();
                re.Text = "BElow are some of the related topics";
                var qnAResults = QnAMaker.QnAFetchter.GetAnswers("#System get #" + selectedIntent + " bot synonyms").Result;
                await context.SendTypingAcitivity();
                ResultCard resultCard = new ResultCard();
                resultCard.CustomQnACard(re, qnAResults);
                await context.PostAsync(re);
                context.Wait(HandleBotSynonyms);
            }
            else if (selectedIntent == null && rk.TopScoringIntent.IntentIntent.Equals("greeting") && rk.TopScoringIntent.Score > Constants.MaxLUIScore)
            {
                PersonalityChatOptions personalityChatOptions = new PersonalityChatOptions(string.Empty, PersonalityChatPersona.Professional);
                PersonalityChatService personalityChatService = new PersonalityChatService(personalityChatOptions);

                var PersonalityChatResults = Task.FromResult<PersonalityChatResults>(await personalityChatService.QueryServiceAsync(rk.Query));
                string botOutput = PersonalityChatResults?.Result.ScenarioList?.FirstOrDefault()?.Responses?.FirstOrDefault() ?? "";

                if (!string.IsNullOrEmpty(botOutput))
                {
                    await context.PostAsync(botOutput);

                    var reply = context.MakeMessage();
                    reply.Attachments = new List<Attachment>();
                    reply.Attachments.Add(ResultCard.ShowGreetingCard());
                    var k = QnAMaker.QnAFetchter.GetAnswers("Get Bot Options").Result;
                    await context.PostAsync(reply);
                }
                else
                {
                    await context.SendTypingAcitivity();
                    await this.HandleBotSynonyms(context, argument);
                }
            }
            else
            {
                response.Text = others;
                await this.HandleBotSynonyms(context, argument);
            }
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

        public async Task HandleBotSynonyms(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var response = await argument;
            await context.SendTypingAcitivity();

            if (!response.Text.Equals(others))
            {
                var k = QnAMaker.QnAFetchter.GetAnswers("#System #Synonyms #ProbingQuestionsDisplay #" + response.Text).Result;
                if (k.Answers!=null && k.Answers[0].Score > Constants.MaxQNAScore)
                {
                    context.PrivateConversationData.SetValue<string>(StateConstants.TopicSynonym, response.Text);
                    await context.PostAsync(k.Answers[0].AnswerAnswer);
                }
                else
                {
                    await this.DoYouWantMoreQna(context, argument);
                    return;
                }
            }
            else
            {
                var k = QnAMaker.QnAFetchter.GetAnswers((context.PrivateConversationData.GetValue<string>(StateConstants.IntentQuery))).Result;
                if (k.Answers!=null && k.Answers[0].Score > Constants.MaxQNAScore)
                {
                    await context.PostAsync(k.Answers[0].AnswerAnswer);
                }
                else
                {
                    await this.DoYouWantMoreQna(context, argument);
                    return;
                }
            }
            ResultCard resultCard = new ResultCard();
            await resultCard.PostAsyncWithConvertToOptionsCard(context, "Did this help you?", issueSolvedCardAction);
            context.Wait(this.DoYouWantMoreQna);
        }

        private static string issueResolved = "Yes, Issue is Resolved";
        private static string doYouWantMore = "No, Issue is not resolved";

        private static string[] issueSolvedCardAction = new string[] { issueResolved, doYouWantMore };
        
        private async Task DoYouWantMoreQna(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            if (response.Text.Equals(issueResolved))
            {
                await this.IssueResolved(context);
                return;
            }

            
            await context.SendTypingAcitivity();

            ResultCard resultCard = new ResultCard();
            await resultCard.PostAsyncWithConvertToOptionsCard(context, "Do you have answers to all the questions displayed earlier", new string[] { "Start Interactive Session" });
            context.Wait(StartInteractiveQuestion);


            //ResultCard resultCard = new ResultCard();
            //await resultCard.PostAsyncWithConvertToOptionsCard(context, "We have suggested possible options", issueSolvedCardAction);

            //context.Wait(this.Completed);
        }

        private async Task StartInteractiveQuestion(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var questionairre = QnAMaker.QnAFetchter.GetAnswers("#System #InteractiveQuestionsDisplay #" + context.PrivateConversationData.GetValue<string>(StateConstants.TopicSynonym)).Result.Answers[0].AnswerAnswer;
            
            if (context.PrivateConversationData.ContainsKey(StateConstants.TopicSynonymInteractiveQuestion))
            {
                var l = context.PrivateConversationData.GetValue<string>(StateConstants.TopicSynonymInteractiveQuestion);
                //questionairre -l;
                var res = questionairre.Replace(l, string.Empty).Split(new string[] { StateConstants.QuestionairreSeparator }, StringSplitOptions.RemoveEmptyEntries);

                if(res == null || res.Length == 0)
                {
                    ResultCard resultCard = new ResultCard();
                    await context.PostAsync("THANK YOU FOR YOUR TIME, BASED ON YOUR ANSWERS I CAN SUGGEST YOU THE BELOW SOLUTION");
                    var intentQueryAnswer = QnAMaker.QnAFetchter.
                        GetAnswers(context.PrivateConversationData.GetValue<string>(StateConstants.IntentQuery)).Result.Answers[0].AnswerAnswer;
                    await context.PostAsync(intentQueryAnswer);
                    await resultCard.PostAsyncWithConvertToOptionsCard(context, "We have suggested possible options", issueSolvedCardAction);
                    context.Wait(this.Completed);
                    return;
                }

                context.PrivateConversationData.SetValue<string>(StateConstants.TopicSynonymInteractiveQuestion, l + StateConstants.QuestionairreSeparator + res[0]);
                await context.PostAsync(res[0]);
                context.Wait(StartInteractiveQuestion);

            }
            else
            {
                await context.PostAsync("Starting interactive session");
                var k = questionairre.Split(new string[] { StateConstants.QuestionairreSeparator }, StringSplitOptions.RemoveEmptyEntries);
                context.PrivateConversationData.SetValue<string>(StateConstants.TopicSynonymInteractiveQuestion, k[0]);
                await context.PostAsync(k[0]);
                context.Wait(StartInteractiveQuestion);
            }
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

            if (context.PrivateConversationData.ContainsKey(StateConstants.Intent))
            {
                var qnAResults = QnAMaker.QnAFetchter.GetAnswers("get " + context.PrivateConversationData.GetValue<string>(StateConstants.Intent) + " last message").Result;
                await context.PostAsync(qnAResults.Answers[0].AnswerAnswer);
            }
            else
            {
                await context.PostAsync("i am not able to understand the question, while i am learning please contact our customer care");
            }
            context.Done("");
        }
    }
}