using Google.Api;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyOrderSupplierMapping.Services.Impl
{
    public class LoggerService:ILoggerService
    {
        private readonly string _projectId;
        private readonly string _logName;

        public LoggerService()
        {
            _projectId = ConfigurationManager.AppSettings["ProjectId"]; ;
            _logName = ConfigurationManager.AppSettings["LogName"]; ;
        }

        /// <summary>
        /// logs each message in gcp log explorer
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logSeverity"></param>
        /// <returns>none</returns>
        public async Task WriteLogEntry(string message, LogSeverity logSeverity)
        {
            var client = await LoggingServiceV2Client.CreateAsync();
            LogName logName = new LogName(_projectId, _logName);
            LogEntry logEntry = new LogEntry
            {
                LogNameAsLogName = logName,
                Severity = logSeverity,
                TextPayload = $"{typeof(Function).FullName} - {message}"
            };
            MonitoredResource resource = new MonitoredResource { Type = "global" };
            IDictionary<string, string> entryLabels = new Dictionary<string, string>
            {
                { "size", "large" },
                { "color", "red" }
            };
            await client.WriteLogEntriesAsync(logName, resource, entryLabels,
                 new[] { logEntry });
        }
    }
}
