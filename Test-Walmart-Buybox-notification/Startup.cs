using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_Walmart_Buybox_notification.Services;
using Test_Walmart_Buybox_notification.Services.Impl;
using Test_Walmart_Buybox_notificationg.Services.Impl;

namespace Test_Walmart_Buybox_notification
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
       services

           .AddScoped<IBigQueryRepo, BigQueryRepo>()
            .AddScoped<ILoggerService, LoggerService>();
         
    }
}
