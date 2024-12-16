using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ShopifyOrderSupplierMapping.Services;
using ShopifyOrderSupplierMapping.Services.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Walmart_Buybox_notification
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
       services

           .AddScoped<IBigQueryRepo, BigQueryRepo>()
           .AddScoped<IOrderSupplierDataService,OrderSupplierDataService>()
            .AddScoped<ILoggerService, LoggerService>()
            .AddScoped<IShopifyService,ShopifyService>()
            .AddScoped<ISupplierMappingService,SupplierMappingService>();
         
    }
}
