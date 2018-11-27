//-----------------------------------------------------------------------
// <copyright file="ITelemetryHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary>
// This file contains ITelemetryHelper interface.
// </summary>
//-----------------------------------------------------------------------

namespace QnAMaker
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// telemetry helper interface
    /// </summary>
    public interface ITelemetryHelper
    {
        /// <summary>
        /// Gets Telemetry client
        /// </summary>
        TelemetryClient TelemetryClient { get; }

        /// <summary>
        /// logs exception to the telemetry
        /// </summary>
        /// <param name="ex">exception to be logged</param>
        /// <param name="message">exception message</param>
        /// <param name="conversationId">conversation id</param>
        void LogException(Exception ex, string message, string conversationId);
        
        /// <summary>
        /// Method to handle exception
        /// </summary>
        /// <param name="ex">exception thrown</param>
        /// <param name="activity">Message Activity</param>
        /// <returns>Task Handler</returns>
        Task HandleExceptionAsync(Exception ex, IActivity activity);

        /// <summary>
        /// logs Events to the telemetry
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="additionalData">Additional Data for event</param>
        /// <param name="context">Dialog Context</param>
        void LogEvent(string eventName, Dictionary<string, string> additionalData, IDialogContext context);

        /// <summary>
        /// logs Events to the telemetry
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="additionalData">Additional Data for event</param>
        /// <param name="activity">Message Activity</param>
        /// <returns>Task Handler</returns>
        Task LogEventAsync(string eventName, Dictionary<string, string> additionalData, IActivity activity);

        /// <summary>
        /// logs a message in telemetry
        /// </summary>
        /// <param name="traceMessage">message to be logged</param>
        /// <param name="moreProps">properties of the message</param>
        /// <param name="context">Message Context</param>
        void LogTrace(string traceMessage, Dictionary<string, string> moreProps, IDialogContext context);

        /// <summary>
        /// logs a message in telemetry
        /// </summary>
        /// <param name="traceMessage">message to be logged</param>
        /// <param name="moreProps">properties of the message</param>
        /// <param name="activity">Message activity</param>
        /// <returns>Task Handler</returns>
        Task LogTraceAsync(string traceMessage, Dictionary<string, string> moreProps, IActivity activity);
    }
}