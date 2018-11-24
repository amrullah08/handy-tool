//-----------------------------------------------------------------------
// <copyright file="SupportQuestionnaireCard.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary>
// This file contains SupportQuestionnaireCard class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Integration.Bot.Cards
{
    using AdaptiveCards;
    using Microsoft.Bot.Connector;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// This class will create various attachments
    /// </summary>
    public class SupportQuestionnaireCard
    {
        /// <summary>
        /// Method returns only Calendar Attachment
        /// </summary>
        /// <returns></returns>
        public Attachment GetCalendarAttachment()
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = this.GetDateBody(),
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction()
                    {
                        Title = "Confirm Date"
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;
        }

        /// <summary>
        /// Method returns attachment for showing email transcript and feedback
        /// </summary>
        /// <returns></returns>
        public Attachment GetEmailFeedbackAttachment()
        {
            string content = (System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("/app_data/adaptivecard.json")));

            var results = AdaptiveCard.FromJson(content);
            var card = results.Card;
            return new Attachment()
            {
                Content = card,
                ContentType = AdaptiveCard.ContentType,
                Name = "Card"
            };
        }

        /// <summary>
        /// Method returns only single submit button
        /// </summary>
        /// <returns></returns>
        public Attachment GetStartNewConversationAttachment()
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction()
                    {
                        Title = "Start new Conversation"
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;
        }

        /// <summary>
        /// Method Returns Questionairre attachment
        /// </summary>
        /// <returns>Returns Questionnaire forum as attachment</returns>
        public Attachment GetQuestionnaireFormAttachment()
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = this.GetBody(),
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction()
                    {
                        Title = "Next"
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;
        }
        /// <summary>
        /// Creates body of adaptive card containing Questionnaire
        /// </summary>
        /// <returns>Returns body of adaptive card</returns>
        private List<AdaptiveElement> GetBody()
        {
            List<AdaptiveElement> body = new List<AdaptiveElement>();
            body.Add(
                                    new AdaptiveTextBlock()
                                    {
                                        Text = "Ford Assistant requires few more details",
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Wrap = true,
                                        Separator = true
                                    });

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("Your name", "your name");
            keyValuePairs.Add("Your Email", "youremail@xyz.com");
            keyValuePairs.Add("your Phone Number", "xx-xxxxxxxx");

            foreach (var item in keyValuePairs)
            {
                body.Add(
                new AdaptiveTextBlock()
                {
                    Text = item.Key,
                    Wrap = true
                });
                body.Add(
                new AdaptiveTextInput()
                {
                    Id = item.Key,
                    Placeholder = item.Value
                });
            }

            body.Add(
            new AdaptiveTextBlock()
            {
                Text = "enter Booking Date",
                Wrap = true
            });
            body.Add(new AdaptiveDateInput()
            {
                Id = "DateEntered",
            });

            return body;
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns body of adaptive card</returns>
        private List<AdaptiveElement> GetDateBody()
        {
            List<AdaptiveElement> body = new List<AdaptiveElement>();

            body.Add(
            new AdaptiveTextBlock()
            {
                Text = "enter Booking Date",
                Wrap = true
            });
            body.Add(new AdaptiveDateInput()
            {
                Id = "DateEntered",
            });

            return body;
        }
    }
}