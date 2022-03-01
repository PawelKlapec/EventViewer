// <copyright file="PipelineQueue.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventViewer.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using EventViewer.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Helper methods for Queue.
    /// </summary>
    public class PipelineQueue
    {
        private static Dictionary<string, string> QueueAttributes
        {
            get
            {
                int maxMessage = 256 * 1024;
                return new Dictionary<string, string>
            {
                {
                    QueueAttributeName.DelaySeconds,
                    TimeSpan.FromSeconds(5).TotalSeconds.ToString()
                },
                {
                    QueueAttributeName.MaximumMessageSize,
                    maxMessage.ToString()
                },
                {
                    QueueAttributeName.MessageRetentionPeriod,
                    TimeSpan.FromDays(4).TotalSeconds.ToString()
                },
                {
                    QueueAttributeName.ReceiveMessageWaitTimeSeconds,
                    TimeSpan.FromSeconds(5).TotalSeconds.ToString()
                },
                {
                    QueueAttributeName.VisibilityTimeout,
                    TimeSpan.FromHours(12).TotalSeconds.ToString()
                },
            };
            }
        }

        /// <summary>
        /// Creates a new subscription to a topic.
        /// </summary>
        /// <param name="client">The initialized Amazon SQS client object, used
        /// to create sqs queue.</param>
        /// <param name="queueName">The name of the queue.</param>
        /// <returns>A string object which includes the queue url.</returns>
        public static async Task<string> CreateAsync(IAmazonSQS client, string queueName)
        {
            var request = new CreateQueueRequest
            {
                Attributes = QueueAttributes,
                QueueName = queueName,
            };

            var response = await client.CreateQueueAsync(request);
            return response.QueueUrl;
        }

        /// <summary>
        /// Listens to SQS queue.
        /// </summary>
        /// <param name="client">The initialized Amazon SQS client object, used
        /// to create sqs queue.</param>
        /// <param name="queueUrl">The url of the queue.</param>
        /// <param name="pretty">Prints prettier events.</param>
        /// <returns>Task.</returns>
        public static async Task ListenToSqsQueue(IAmazonSQS client, string queueUrl, bool pretty = false)
        {
            JObject json = new ();

            while (true)
            {
                var response = await ReceiveMessage(client, queueUrl);
                response.Messages.ForEach(m =>
                {
                    CbcEvent? cbcEvent = JsonConvert.DeserializeObject<CbcEvent>(m.Body);

                    if (cbcEvent != null)
                    {
                        Console.WriteLine($"{cbcEvent.ToString(pretty)}");
                    }
                });

                Thread.Sleep(500);
            }
        }

        private static async Task<ReceiveMessageResponse> ReceiveMessage(IAmazonSQS client, string queueUrl)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 5,
                WaitTimeSeconds = (int)TimeSpan.FromSeconds(5).TotalSeconds,
            };

            return await client.ReceiveMessageAsync(request);
        }
    }
}