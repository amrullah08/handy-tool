namespace RavePOCBot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using System.Web.Http;
    using Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Connector;
    using Services;
    using RavePOCBot.Cards;
    using RavePOCBot.Common;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static readonly bool IsSpellCorrectionEnabled = bool.Parse(WebConfigurationManager.AppSettings["IsSpellCorrectionEnabled"]);

        private readonly BingSpellCheckService spellService = new BingSpellCheckService();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                if (IsSpellCorrectionEnabled)
                {
                    try
                    {
                        activity.Text = await this.spellService.GetCorrectedTextAsync(activity.Text);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
                }

                await Conversation.SendAsync(activity, () => new RootDialog());
                
                //var rootModelAttribute = new LuisModelAttribute(WebConfigurationManager.AppSettings["LuisModelId"], WebConfigurationManager.AppSettings["LuisSubscriptionKey"]);
                //await Conversation.SendAsync(activity, () => new RootLuisDialog(rootModelAttribute));
            }
            else
            {
                this.HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                if (message.Type == ActivityTypes.ConversationUpdate)
                {
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels

                    // Note: Add introduction here:
                    IConversationUpdateActivity iConversationUpdated = message as IConversationUpdateActivity;
                    if (iConversationUpdated != null)
                    {
                        ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));

                        foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                        {
                            // if the bot is added, then 
                            if (member.Id == iConversationUpdated.Recipient.Id)
                            {
                                var reply = ((Activity)iConversationUpdated).CreateReply();
                                reply.Attachments = new List<Attachment>();

                                reply.Attachments.Add(ResultCard.ShowGreetingCard());
                                var k = QnAMaker.QnAFetchter.GetAnswers("Get Bot Options").Result;
                                //reply.SuggestedActions = ResultCard.GetSuggestedQnAActions(k.Answers[0].AnswerAnswer.Split(','));
                                connector.Conversations.ReplyToActivityAsync(reply);
                            }
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}