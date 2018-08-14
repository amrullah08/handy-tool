using Microsoft.Bot.Connector;
using SharePointPOCLib;
using System;
using System.Collections.Generic;
using AdaptiveCards;

namespace SharePointPOCBot.Cards
{
    public class ResultCard
    {
        public void RenderLegalDocuments(IMessageActivity message, List<LegalDocuments> legalDocuments)
        {
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment>();

            foreach (var cardContent in legalDocuments)
            {
                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = $"{cardContent.FileUrl}",
                    Type = "openUrl",
                    Title = cardContent.Title
                };

                cardButtons.Add(plButton);
                var presrvData = (cardContent.OnPreservationHold) ? "Case is on Legal Hold" : "Case is not on Legal Hold";
                HeroCard plCard = new HeroCard()
                {
                    Title = $"{cardContent.Title}",
                    Subtitle = $"{presrvData}",
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                message.Attachments.Add(plAttachment);
            }
        }

        public void RenderLitigationDocuments(IMessageActivity message, List<LitigationDocument> legalDocuments)
        {
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment>();

            foreach (var cardContent in legalDocuments)
            {
                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = $"{cardContent.FileUrl}",
                    Type = "openUrl",
                    Title = cardContent.Title
                };

                cardButtons.Add(plButton);
                var caseData = (cardContent.ActiveCase) ? "Case is Active " : "Case is not Active";
                HeroCard plCard = new HeroCard()
                {
                    Title = $"{cardContent.Title}",
                    Subtitle = $"{caseData}",
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                message.Attachments.Add(plAttachment);
            }
        }

        public void RenderInternalDocuments(IMessageActivity message, List<InternalInvestigationDocument> legalDocuments)
        {
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment>();

            foreach (var cardContent in legalDocuments)
            {
                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = $"{cardContent.FileUrl}",
                    Type = "openUrl",
                    Title = cardContent.Title
                };

                cardButtons.Add(plButton);

                HeroCard plCard = new HeroCard()
                {
                    Title = $"{cardContent.Title}",
                    Subtitle = $"Year of Investigation {cardContent.YearOfInternalInvestigation}",
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                message.Attachments.Add(plAttachment);
            }
        }
        public void Card(IMessageActivity message)
        {
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment>();

            Dictionary<string, string> cardContentList = new Dictionary<string, string>();
            cardContentList.Add("PigLatin", "https://<ImageUrl1>");
            cardContentList.Add("Pork Shoulder", "https://<ImageUrl2>");
            cardContentList.Add("Bacon", "https://<ImageUrl3>");

            foreach (KeyValuePair<string, string> cardContent in cardContentList)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: cardContent.Value));

                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = $"https://en.wikipedia.org/wiki/{cardContent.Key}",
                    Type = "openUrl",
                    Title = "WikiPedia Page"
                };

                cardButtons.Add(plButton);

                HeroCard plCard = new HeroCard()
                {
                    Title = $"I'm a hero card about {cardContent.Key}",
                    Subtitle = $"{cardContent.Key} Wikipedia Page",
                    Images = cardImages,
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                message.Attachments.Add(plAttachment);
            }
        }


        public void CustomCard(IMessageActivity message)
        {
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment>();

            Dictionary<string, string> cardContentList = new Dictionary<string, string>();
            cardContentList.Add("PigLatin", "https://<ImageUrl1>");
            cardContentList.Add("Pork Shoulder", "https://<ImageUrl2>");
            cardContentList.Add("Bacon", "https://<ImageUrl3>");

            foreach (KeyValuePair<string, string> cardContent in cardContentList)
            {
                List<AdaptiveElement> items = new List<AdaptiveElement>();
                items.Add(new AdaptiveColumnSet()
                {
                    Columns = new List<AdaptiveColumn>()
                            {
                                new AdaptiveColumn
                                {
                                    Width = "6"
                                },
                        new AdaptiveColumn()
                                {
                                    Items = new List<AdaptiveElement>()
                                            {
                                                new AdaptiveTextBlock()
                                                {
                                                    Text = cardContent.Key,
                                                    Weight = AdaptiveTextWeight.Bolder,
                                                    Size = AdaptiveTextSize.Small,
                                                    Wrap = true
                                                }
                                            }
                                }
                    }
                });


                AdaptiveCard card = new AdaptiveCard()
                {
                    Body = items
                };
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };

                message.Attachments.Add(attachment);
            }
        }

        public static Attachment ShowGreetingCard()
        {
            AdaptiveContainer adaptiveContainer = new AdaptiveContainer();
            AdaptiveColumnSet adaptiveColumnSet = new AdaptiveColumnSet();
            AdaptiveColumn adaptiveColumn = new AdaptiveColumn()
            {
                Width = "auto",
                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage()
                                    {
                                        Url = new Uri("https://www.chevron.com/Assets/images/hallmark.png")
                                    }
                                }
            };
            adaptiveColumnSet.Columns.Add(adaptiveColumn);
            adaptiveColumn = new AdaptiveColumn()
            {
                Width = "stretch",
                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        Text = "I am The "+ System.Configuration.ConfigurationManager.AppSettings["Company"] + " Bot, One place to find all your legal, litigation and active investigation cases",
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Wrap = true                                        
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Text = "Please choose the categories below or ask questions",
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Size = AdaptiveTextSize.Small,
                                        Wrap = true
                                    }
                                }
            };
            adaptiveColumnSet.Columns.Add(adaptiveColumn);
            adaptiveContainer.Items.Add(adaptiveColumnSet);

            AdaptiveCard adaptiveCard = new AdaptiveCard();
            adaptiveCard.Body.Add(adaptiveContainer);

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard
            };
            return attachment;
        }


        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
                
            };

            return heroCard.ToAttachment();
        }

        public static Attachment GetThumbnailCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }

        public static SuggestedActions GetSuggestedActions()
        {
            return new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title = "preservation hold", Type=ActionTypes.PostBack, Value=$"How many documents are on preservation hold for this SP Online site?" },
                    new CardAction(){ Title = "active litigation cases", Type=ActionTypes.PostBack, Value=$"How many active litigation cases did we receive between 1/1/2017 to 8/12/2018?" },
                    new CardAction(){ Title = "internal investigation cases", Type=ActionTypes.PostBack, Value=$"How many substantiated internal investigation cases do we have in 2018?" }
                }
            };
        }

    }
}