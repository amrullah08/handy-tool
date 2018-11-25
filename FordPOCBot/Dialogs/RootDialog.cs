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
            await context.SendTypingAcitivity();
            dynamic message = await argument;
            var formValue = message.Value;
            var formValueObject = JObject.FromObject(formValue);

            IList<string> partnerFilledData = new List<string>();
            var keyValuePairEnum = (IEnumerable<KeyValuePair<string, JToken>>)formValueObject;
            await context.PostAsync("welcome " + keyValuePairEnum.First().Value.ToString());


            SupportQuestionnaireCard supportQuestionnaireCard = new SupportQuestionnaireCard();

            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
            reply.Attachments.Add(supportQuestionnaireCard.GetCalendarAttachment());

            reply.Attachments.Add(supportQuestionnaireCard.GetEmailFeedbackAttachment());
            reply.Attachments.Add(supportQuestionnaireCard.GetStartNewConversationAttachment());

            await context.PostAsync(reply);

            context.Wait(DateSelected);
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