using Google.Api.Gax.ResourceNames;
using Google.Cloud.Tasks.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Configuration;

namespace ShopifyOrderSupplierMapping.Modals
{
    public class Tasks
    {
        /// <summary>
        /// enqueue task in task queue
        /// </summary>
        /// <param name="queue">GCP queue name</param>
        /// <param name="target"> deployed function url </param>
        /// <param name="projectId">GCP porject id</param>
        /// <param name="location">GCP location</param>
        /// <returns> Task </returns>
        public static async System.Threading.Tasks.Task<Task> PushAsync(string queue, string target, string projectId, string location, string body, int bufferTime = 10)
        {
            var client = await CloudTasksClient.CreateAsync();
            var task = new Task
            {
                HttpRequest = new HttpRequest
                {
                    Url = target,
                    HttpMethod = HttpMethod.Post,
                    Body = ByteString.CopyFromUtf8(body)
                },
                ScheduleTime = Timestamp.FromDateTime(DateTime.UtcNow.AddSeconds(bufferTime))
            };

            var queueName = new QueueName(projectId, location, queue);
            var taskRequest = new CreateTaskRequest
            {
                ParentAsQueueName = queueName,
                Task = task
            };
            var respones = await client.CreateTaskAsync(taskRequest);
            return respones;
        }

    }
}
