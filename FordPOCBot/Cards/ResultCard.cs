﻿using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using FordPOCBot.Common;
using FordPOCBot.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaWikiNET;

namespace FordPOCBot.Cards
{
    public class ResultCard
    {
        public static Attachment GetVideoCard(string title, string subtitle, string text, MediaUrl cardImage)
        {
            var heroCard = new VideoCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Media = new List<MediaUrl>()
            };

            return heroCard.ToAttachment();
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
                                        Size= AdaptiveImageSize.Medium,
                                        Url = new Uri("https://pbs.twimg.com/profile_images/892474147569377281/e60htCEm_400x400.jpg")
                                    }
                                }
            };
            adaptiveColumnSet.Columns.Add(adaptiveColumn);

            adaptiveColumn = new AdaptiveColumn()
            {
                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        Text = FordResources.GreetUser,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Wrap = true,
                                        Size = AdaptiveTextSize.Medium
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Text = FordResources.BotGreeting,
                                        Weight = AdaptiveTextWeight.Bolder,
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

        public Attachment FeedBack()
        {

            List<AdaptiveElement> body = new List<AdaptiveElement>();
            body.Add(
                                    new AdaptiveTextBlock()
                                    {
                                        Text = "Can you please tell me more about your experience??",
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Wrap = true,
                                        Separator = true
                                    });

            //foreach (var item in keyValuePairs)
            {
                body.Add(
                new AdaptiveTextInput()
                {
                    Id = "enter your feedback here",
                    Placeholder = "enter your feedback here",
                    IsMultiline=true 
                });
            }


            AdaptiveCard card = new AdaptiveCard()
            {
                Body = body,
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction()
                    {
                        Title = "Submit Feedback"
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

        public void Card(IMessageActivity message)
        {
            //MediaWiki mediaWiki = new MediaWiki("");
            //MediaWikiNET.Models.SearchRequest searchRequest = new MediaWikiNET.Models.SearchRequest("");
            //mediaWiki.Search(searchRequest);

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

        public async Task PostAsyncWithConvertToOptionsCard(IDialogContext context, string title, string[] options)
        {
            var re = context.MakeMessage();
            if (!string.IsNullOrEmpty(title))
                re.Text = title;
            this.ConvertToOptionsCard(re, options, null);
            await context.PostAsync(re);
        }

        public void ConvertToOptionsCard(IMessageActivity message, string[] options,string[] urls)
        {
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment>();

            int i = 0;

            foreach (var cardContent in options)
            {
                List<AdaptiveElement> items = new List<AdaptiveElement>(){
                                    new AdaptiveImage()
                                    {
                                        Size= AdaptiveImageSize.Medium,
                                        Url = new Uri(urls[i])
                                    }
                                };


                AdaptiveCard card = new AdaptiveCard()
                {
                    Body = items,
                    BackgroundImage = new Uri(urls[i++])
                };
                card.Actions.Add(new AdaptiveSubmitAction()
                {
                    Title = "",//cardContent,
                    Data = cardContent
                });
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };

                message.Attachments.Add(attachment);
            }
        }

        public void CustomQnACard(IMessageActivity message, QnAResult qnAResult)
        {
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            message.Attachments = new List<Attachment>();

            foreach (var cardContent in qnAResult.Answers[0].AnswerAnswer.Replace(" , ", ",").Replace(" ,", ",").Replace(", ", ",").Split(','))
            {
                List<AdaptiveElement> items = new List<AdaptiveElement>();

                AdaptiveCard card = new AdaptiveCard()
                {
                    Body = items
                };
                card.Actions.Add(new AdaptiveSubmitAction()
                {
                    Title = cardContent,
                    Data = cardContent
                });
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };

                message.Attachments.Add(attachment);
            }
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
    }
}