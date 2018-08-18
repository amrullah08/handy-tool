//-----------------------------------------------------------------------
// <copyright file="DialogContextHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary>
// This file contains DialogContextHelper class.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Integration.Bot.Helpers
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Extension for DialogContext
    /// </summary>
    public static class DialogContextHelper
    {
        /// <summary>
        /// Typing Activity extension for DialogContext
        /// </summary>
        /// <param name="context">dialog context</param>
        /// <returns>sends typing activity</returns>
        public static async Task SendTypingAcitivity(this IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.Text = null;
            reply.Type = ActivityTypes.Typing;
            await context.PostAsync(reply);
        }

    }
}