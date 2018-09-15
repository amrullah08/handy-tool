// <copyright file="QnADialog.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary>
// This file contains QnAMaker class.
// </summary>
//-----------------------------------------------------------------------

namespace RavePOCBot.Dialogs
{
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// dialog for connecting to Question and answer service
    /// </summary>
    [Serializable]
    public class QnADialog : QnAMakerDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QnADialog"/> class.
        /// </summary>
        /// <param name="service">question and answer service</param>
        public QnADialog(IQnAService service) : base(service)
        {
        }

        /// <summary>
        /// Override method
        /// </summary>
        /// <param name="context">Dialog context</param>
        /// <param name="message">user message</param>
        /// <param name="result">result returned from question and answer service</param>
        /// <returns>returns nothing</returns>
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            context.Done<IMessageActivity>(null);
        }
    }
}