using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace EventViewer.Pipeline
{
    public class Queue
    {
        /// <summary>
        /// Creates a new subscription to a topic.
        /// </summary>
        /// <param name="client">The initialized Amazon SQS client object, used
        /// to create sqs queue.</param>
        /// <param name="queueName">The name of the queue.</param>
        /// <returns>A SubscribeResponse object which includes the queue url.</returns>
        public static async Task<SubscribeResponse> CreateAsync(IAmazonSQSClient client, string queueName)
        {
            var request = new CreateQueueRequest
            {
                Attributes = QueueAttributes,
                QueueName = queueName,
            };

            return await client.CreateQueueAsync(request);
        }

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
    }
}