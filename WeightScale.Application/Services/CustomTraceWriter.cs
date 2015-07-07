/// <summary>
/// Summary description for CustomTraceWriter
/// </summary>
namespace WeightScale.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http.Tracing;
    using log4net;

    public class CustomTraceWriter : ITraceWriter
    {
        private readonly ILog logger;

        public CustomTraceWriter(ILog loggerParam)
        {
            this.logger = loggerParam;
        }

        /// <summary>
        /// Traces the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="category">The category.</param>
        /// <param name="level">The level.</param>
        /// <param name="traceAction">The trace action.</param>
        public void Trace(HttpRequestMessage request, string category,
            TraceLevel level, Action<TraceRecord> traceAction)
        {
            TraceRecord traceRecord = new TraceRecord(request, category, level);
            traceAction(traceRecord);
            var message = GetTrace(traceRecord);
            LogMessage(message, traceRecord.Level);
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        private void LogMessage(string message, TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Debug:
                    this.logger.Debug(message);
                    break;
                case TraceLevel.Error:
                    this.logger.Error(message);
                    break;
                case TraceLevel.Fatal:
                    this.logger.Fatal(message);
                    break;
                case TraceLevel.Info:
                    this.logger.Info(message);
                    break;
                case TraceLevel.Off:
                    break;
                case TraceLevel.Warn:
                    this.logger.Warn(message);
                    break;
                default:
                    break;
            }
        }

        private string GetTrace(TraceRecord traceRecord)
        {
            if (traceRecord == null)
            {
                return "No trace record";
            }
            return
                String.Format(
                    "{0} {1}: Category: {2}; Level: {3}; Kind: {4}; Operator: {5}; Operation: {6}; Status: {7}; TimeStamp: {8}; Message: {9}; Exception: {10}",
                    traceRecord.Request != null ? traceRecord.Request.Method.ToString() : "No method",
                    traceRecord.Request != null ? traceRecord.Request.RequestUri.ToString() : "No Uri",
                    traceRecord.Category,
                    traceRecord.Level,
                    traceRecord.Kind,
                    traceRecord.Operator,
                    traceRecord.Operation,
                    traceRecord.Status,
                    traceRecord.Timestamp,
                    traceRecord.Message != null ? traceRecord.Message.Replace("\r\n", " ") : @"""No message""",
                    traceRecord.Exception != null
                        ? traceRecord.Exception.GetBaseException().Message + traceRecord.Exception.GetBaseException().StackTrace
                        : string.Empty
                );
        }
    }
}
