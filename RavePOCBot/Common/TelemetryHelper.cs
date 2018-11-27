namespace QnAMaker
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Autofac;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using System.Configuration;

    /// <summary>
    /// class to handle exceptions
    /// </summary>
    public class TelemetryHelper : ITelemetryHelper
    {

        /// <summary>
        /// Initializes a new instance of the TelemetryHelper class
        /// </summary>
        public TelemetryHelper()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the telemetry insights client
        /// </summary>
        public TelemetryClient TelemetryClient { get; set; }

        /// <summary>
        /// logs exception to the telemetry
        /// </summary>
        /// <param name="ex">exception to be logged</param>
        /// <param name="message">exception message</param>
        /// <param name="conversationId">conversation id</param>
        public void LogException(Exception ex, string message, string conversationId)
        {
            Dictionary<string, string> props = new Dictionary<string, string>
            {
                { "Message", message },
                { "ConversationId", conversationId }
            };

            this.TelemetryClient.TrackException(ex, props);
            this.TelemetryClient.Flush();
        }

        /// <summary>
        /// logs Events to the telemetry
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="additionalData">Additional Data for event</param>
        /// <param name="context">conversation context</param>
        public void LogEvent(string eventName, Dictionary<string, string> additionalData, IDialogContext context)
        {
            Dictionary<string, string> props = new Dictionary<string, string>
            {
                { TelemetryConstants.TimeStamp, DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture) },
            };


            if (additionalData != null && additionalData.Count > 0)
            {
                foreach (var i in additionalData)
                {
                    props.Add(i.Key, i.Value);
                }
            }

            this.TelemetryClient.TrackEvent(eventName, props);
            this.TelemetryClient.Flush();
        }

        /// <summary>
        /// logs Events to the telemetry
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="additionalData">Additional Data for event</param>
        /// <param name="activity">Message Activity</param>
        /// <returns>Task Handler</returns>
        public async Task LogEventAsync(string eventName, Dictionary<string, string> additionalData, IActivity activity)
        {
            Dictionary<string, string> additionalProps = new Dictionary<string, string>
            {
                { TelemetryConstants.TimeStamp, DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture) },
            };


            if (additionalData != null && additionalData.Count > 0)
            {
                foreach (var i in additionalData)
                {
                    additionalProps.Add(i.Key, i.Value);
                }
            }

            this.TelemetryClient.TrackEvent(eventName, additionalProps);
            this.TelemetryClient.Flush();
        }

        /// <summary>
        /// Handle Exception
        /// </summary>
        /// <param name="ex">Exception thrown</param>
        /// <param name="additionalProperties">Additional properties for insights</param>
        public void HandleException(Exception ex, Dictionary<string, string> additionalProperties)
        {
            this.TelemetryClient.TrackException(ex, additionalProperties);
        }

        /// <summary>
        /// Method to handle exception
        /// </summary>
        /// <param name="ex">exception thrown</param>
        /// <param name="activity">Message Activity</param>
        /// <returns>Task handler</returns>
        public async Task HandleExceptionAsync(Exception ex, IActivity activity)
        {
            Dictionary<string, string> additionalProps = new Dictionary<string, string>
            {
                { TelemetryConstants.TimeStamp, DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture) },
            };


            this.HandleException(ex, additionalProps);
            try
            {
                string filePath = string.Format(CultureInfo.InvariantCulture, @"{0}\bin\Error_{1:yyyyMMdd}.txt", HttpRuntime.AppDomainAppPath, DateTime.UtcNow);

                if (!File.Exists(filePath))
                {
                    using (var fs = File.Create(filePath))
                    {
                    }
                }

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    string format = string.Format("Message : {0}{1} Exception {2}{1} Date : {3}", ex.Message, Environment.NewLine, ex, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine(format);
                    writer.WriteLine(Environment.NewLine + new string('-', 200) + Environment.NewLine);
                }
            }
            catch (Exception exc)
            {
                this.LogException(exc, exc.Message, activity.Conversation.Id);
            }
        }

        /// <summary>
        /// logs a message in telemetry
        /// </summary>
        /// <param name="traceMessage">message to be logged</param>
        /// <param name="moreProps">properties of the message</param>
        /// <param name="context">Message Context</param>
        public void LogTrace(string traceMessage, Dictionary<string, string> moreProps, IDialogContext context)
        {
            Dictionary<string, string> additionalProps = new Dictionary<string, string>
            {
                { TelemetryConstants.TimeStamp, DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture) },
            };


            if (moreProps != null && moreProps.Count > 0)
            {
                foreach (var i in moreProps)
                {
                    additionalProps.Add(i.Key, i.Value);
                }
            }

            this.TelemetryClient.Context.Operation.Name = traceMessage;

            this.TelemetryClient.TrackTrace(traceMessage, additionalProps);
            this.TelemetryClient.Flush();
        }

        /// <summary>
        /// logs a message in telemetry
        /// </summary>
        /// <param name="traceMessage">message to be logged</param>
        /// <param name="moreProps">properties of the message</param>
        /// <param name="activity">Message activity</param>
        /// <returns>Task Handler</returns>
        public async Task LogTraceAsync(string traceMessage, Dictionary<string, string> moreProps, IActivity activity)
        {
            Dictionary<string, string> additionalProps = new Dictionary<string, string>
            {
                { TelemetryConstants.TimeStamp, DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture) },
            };


            if (moreProps != null && moreProps.Count > 0)
            {
                foreach (var i in moreProps)
                {
                    additionalProps.Add(i.Key, i.Value);
                }
            }

            this.TelemetryClient.Context.Operation.Name = traceMessage;

            this.TelemetryClient.TrackTrace(traceMessage, additionalProps);
            this.TelemetryClient.Flush();
        }

        /// <summary>
        /// Initializes bot instrumentation
        /// </summary>
        private void Initialize()
        {
            string botInstrumentationKey = null;
            try
            {
                botInstrumentationKey = ConfigurationManager.AppSettings["BotInstrumentationKey"];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            if (string.IsNullOrEmpty(botInstrumentationKey))
            {
                this.TelemetryClient = new TelemetryClient();
            }
            else
            {
                this.TelemetryClient = new TelemetryClient(Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active)
                {
                    InstrumentationKey = botInstrumentationKey
                };
            }
        }
    }
}