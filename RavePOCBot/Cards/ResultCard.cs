using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using AdaptiveCards;
using RavePOCBot.Dialogs;
using RavePOCBot.Common;

namespace RavePOCBot.Cards
{
    public class ResultCard
    {
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


        public void CustomCard(IMessageActivity message, WebPages webPages)
        {
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment>();

        
            foreach (var cardContent in webPages.value)
            {
                List<AdaptiveElement> items = new List<AdaptiveElement>();
                items.Add(new AdaptiveColumnSet()
                {
                    Columns = new List<AdaptiveColumn>()
                            {
                        new AdaptiveColumn()
                                {
                                    Items = new List<AdaptiveElement>()
                                            {
                                                new AdaptiveTextBlock()
                                                {
                                                    Text = cardContent.name,
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
                card.Actions.Add(new AdaptiveOpenUrlAction()
                {
                    Url = new Uri(cardContent.url),
                    Title = "Click me To Open"
                });
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
                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage()
                                    {
                                        Size= AdaptiveImageSize.Auto,
                                        Url = new Uri("https://rave.office.net/api/download/publicBlob?fileURL=https%3a%2f%2fwebrave.blob.core.windows.net%2fpublic%2fimages%2fRave_DJ_2.gif")

                                    }
                                }
            };
            adaptiveColumnSet.Columns.Add(adaptiveColumn);

            //adaptiveColumn = new AdaptiveColumn()
            //{
            //    Items = new List<AdaptiveElement>()
            //                    {
            //                        new AdaptiveTextBlock()
            //                        {
            //                            Text=""
            //                        }
            //                    }
            //};
            //adaptiveColumnSet.Columns.Add(adaptiveColumn);

            //adaptiveColumn = new AdaptiveColumn()
            //{
            //    Items = new List<AdaptiveElement>()
            //                    {
            //                        new AdaptiveTextBlock()
            //                        {
            //                            Text=""
            //                        }
            //                    }
            //};
            //adaptiveColumnSet.Columns.Add(adaptiveColumn);

            adaptiveColumn = new AdaptiveColumn()
            {
                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        Text = "I am The "+ System.Configuration.ConfigurationManager.AppSettings["Company"] + " Assistant. One place to find all your Performance, Troubleshooting issues.",
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Wrap = true                                        
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Text = "How can i help you?",
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
                    new CardAction(){ Title = "Trobuleshootin", Type=ActionTypes.PostBack, Value=$"Trobuleshootin" },
                    new CardAction(){ Title = "Performance", Type=ActionTypes.PostBack, Value=$"performacne" }
                }
            };
        }

        public static SuggestedActions GetSuggestedQnAActions(string[] result)
        {
            var k = new SuggestedActions()
            {
                Actions = new List<CardAction>()
            };
            foreach (var r in result)
            {
                k.Actions.Add(new CardAction() { Title = r, Type = ActionTypes.PostBack, Value = $"{r}" });
            }
            return k;
        }

        public static SuggestedActions GetSuggestedQnAActions(QnAResult qnAResult)
        {
            var k = new SuggestedActions()
            {
                Actions = new List<CardAction>()
            };
            foreach(var r in qnAResult.Answers)
            {
                k.Actions.Add(new CardAction() { Title = r.AnswerAnswer, Type = ActionTypes.PostBack, Value = $"{r.AnswerAnswer}" });
            }
            return k;
        }
    }
}