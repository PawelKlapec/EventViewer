using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace EventViewer.Pipeline
{
    public static class Topic
    {
        /// <summary>
        /// Creates a new subscription to a topic.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to create an Amazon SNS subscription.</param>
        /// <param name="topicArn">The ARN of the topic to subscribe to.</param>
        /// <returns>A SubscribeResponse object which includes the subscription
        /// ARN for the new subscription.</returns>
        public static async Task<SubscribeResponse> SubscribeQueueAsync(
            IAmazonSimpleNotificationService client,
            string topicArn,
            string queueUrl)
        {
            SubscribeRequest request = new SubscribeRequest()
            {
                TopicArn = topicArn,
                ReturnSubscriptionArn = true,
                Endpoint = queueUrl,
                Protocol = "sqs"
            };

            var response = await client.SubscribeQueue(request);

            return response;
        }

        /// <summary>
        /// Given the ARN for an Amazon SNS subscription, this method deletes
        /// the subscription.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to delete an Amazon SNS subscription.</param>
        /// <param name="subscriptionArn">The ARN of the subscription to delete.</param>
        public static async Task UnsubscribeAsync(
            IAmazonSimpleNotificationService client,
            string subscriptionArn)
        {
            var response = await client.UnsubscribeAsync(subscriptionArn);
        }

        /// <summary>
        /// Displays the list of Amazon SNS Topic ARNs.
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to create an Amazon SNS subscription.</param>
        /// </summary>
        public static async Task ListTopicsAsync(IAmazonSimpleNotificationService client)
        {
            Console.WriteLine($"Topics list\n --------------");

            var index = 1;
            var topicList = await GetTopicListAsync(client);

            foreach (var topic in topicList)
            {
                Console.WriteLine($"${index} --");
                Console.WriteLine($"{topic.Name}");
                Console.WriteLine($"{topic.TopicArn}");
                index++;
            }
        }

        private static async Task<List<Topic>> GetTopicListAsync(IAmazonSimpleNotificationService client)
        {
            var topics = new List<Topic>();
            // If there are more than 100 Amazon SNS topics, the call to
            // ListTopicsAsync will return a value to pass to the
            // method to retrieve the next 100 (or less) topics.
            string nextToken = string.Empty;

            do
            {
                var response = await client.ListTopicsAsync(nextToken);
                topics.AddRange(response.Topics);
                nextToken = response.NextToken;
            }
            while (!string.IsNullOrEmpty(nextToken));

            return topics;
        }
    }
}