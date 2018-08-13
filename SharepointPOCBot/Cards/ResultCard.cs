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

                HeroCard plCard = new HeroCard()
                {
                    Title = $"{cardContent.Title}",
                    Subtitle = $"{cardContent.OnPreservationHold}",
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

                HeroCard plCard = new HeroCard()
                {
                    Title = $"{cardContent.Title}",
                    Subtitle = $"{cardContent.ActiveCase}",
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
                    Subtitle = $"{cardContent.YearOfInternalInvestigation}",
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
    }
}