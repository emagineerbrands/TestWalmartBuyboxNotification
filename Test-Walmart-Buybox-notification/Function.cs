using Google.Api;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Google.Cloud.Logging.Type;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ShopifyOrderSupplierMapping.Modals;
using ShopifyOrderSupplierMapping.Services;
using ShopifyOrderSupplierMapping.Services.Impl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Test_Walmart_Buybox_notification;

namespace ShopifyOrderSupplierMapping
{
    [FunctionsStartup(typeof(Startup))]
    public class Function : IHttpFunction
    {
        private readonly ILoggerService _loggerService;
        private readonly IBigQueryRepo _bigQueryRepo;

        private readonly string _projectId;
        private readonly string _location;
        private string _target;
        private readonly string _WalmartTestQueue;

        public Function(ILoggerService loggerService, IBigQueryRepo bigQueryRepo)
        {
            _loggerService = loggerService;
            _bigQueryRepo = bigQueryRepo;


            _WalmartTestQueue = ConfigurationManager.AppSettings["WalmartBuyboxTestQueue"];
            _target = ConfigurationManager.AppSettings["ServiceUrl"];
            _projectId = ConfigurationManager.AppSettings["ProjectId"];
            _location = ConfigurationManager.AppSettings["Location"];
        }


        /// <summary>
        ///  job to receive order payload from shopify order webhook, map minimal cost supplier and store in db 
        /// </summary>
        /// <param name="context">The HTTP context, containing the request and the response.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleAsync(HttpContext context)
        {
            Console.WriteLine("HandleAsync called");
            Console.WriteLine($"request method - {context.Request.Method}");
            Console.WriteLine($"request path - {context.Request.Path}");


            if (context.Request.Method == HttpMethods.Post && context.Request.Path == "/WalmartBuyboxNotificationTest")
            {
                Console.WriteLine("Method called");
                string requestBody;
                using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                Console.WriteLine($"requestbody - {requestBody}");
                await _loggerService.WriteLogEntry($"requestbody - {requestBody}", LogSeverity.Info);

                await Tasks.PushAsync(_WalmartTestQueue, _target, _projectId, _location, requestBody);


                //await _supplierMappingService.CustmoreFirstorder(requestBody);
                //await context.Response.WriteAsync("Order Captured!");

            }


            if (context.Request.Method == HttpMethods.Post && context.Request.Path == "/ProcessPayload")
            {
                try
                {
                    string requestBody;
                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }


                    ConcurrentQueue<BigQueryInsertRow> bigQueryInsertRows = new();
                    var bqRow = new BigQueryInsertRow
                    {

                        { "json_data", requestBody },

                    };
                    bigQueryInsertRows.Enqueue(bqRow);

                    BigQueryInsertResults bigQueryInsertResults = await _bigQueryRepo.InsertRows(ConfigurationManager.AppSettings["Dataset"],
                            ConfigurationManager.AppSettings["Test_Walmart_Buybox_notificationTable"], bigQueryInsertRows);
                }
                catch (Exception ex)
                {
                    // Log the error and respond with failure
                    Console.WriteLine($"Error: {ex.Message}");
                    await context.Response.WriteAsync("An error occurred while processing the request.");
                    throw;
                }
            }

        }

        private ConcurrentQueue<BigQueryInsertRow> ParseRequestToBigQueryRows(string requestBody)
        {
            throw new NotImplementedException();
        }
    }
}