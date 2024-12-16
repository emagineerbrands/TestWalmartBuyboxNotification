using Google.Api;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using ShopifyOrderSupplierMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyOrderSupplierMapping.Services
{
    public interface ILoggerService
    {
        Task WriteLogEntry(string message, LogSeverity logSeverity);
    }
}

