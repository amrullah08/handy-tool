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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using AdaptiveCards;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Questionnaire forum 
    /// </summary>
    public class SupportQuestionnaireCard 
    {
        /// <summary>
        /// Method for creating Questionnaire forum as attachment
        /// </summary>
        /// <returns>Returns Questionnaire forum as attachment</returns>
        public Attachment QuestionnaireForm()
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

        public Attachment OnlyDateForm()
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
        /// Creates body of adaptive card containing Questionnaire
        /// </summary>
        /// <returns>Returns body of adaptive card</returns>
        private List<AdaptiveElement> GetBody()
        {
            List<AdaptiveElement> body = new List<AdaptiveElement>();
            body.Add(
                                    new AdaptiveTextBlock()
                                    {
                                        Text = "" + System.Configuration.ConfigurationManager.AppSettings["Company"] + " Assistant requires few more details",
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
        /// Creates body of adaptive card containing Questionnaire
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


        public Attachment EndConversation()
        {

            AdaptiveCard card = new AdaptiveCard()
            {
                Body = this.GetDateBody(),
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction()
                    {
                        Title = "Confirm Date",
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

        public Attachment GetEndOfConversatoin()
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
        public Attachment OnlySubmitForm()
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
    }
}