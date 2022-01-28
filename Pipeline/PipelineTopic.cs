// <copyright file="PipelineTopic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventViewer.Pipeline
{
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;

    /// <summary>
    /// Helper methods for topic.
    /// </summary>
    public static class PipelineTopic
    {
        /// <summary>
        /// Creates a new subscription to a topic.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to create an Amazon SNS subscription.</param>
        /// <param name="topicArn">The ARN of the topic to subscribe to.</param>
        /// <param name="queueUrl">The queue url.</param>
        /// <returns>A SubscribeResponse object which includes the subscription
        /// ARN for the new subscription.</returns>
        public static async Task<SubscribeResponse> SubscribeQueueAsync(
            IAmazonSimpleNotificationService client,
            string topicArn,
            string queueUrl)
        {
            SubscribeRequest request = new ()
            {
                TopicArn = topicArn,
                ReturnSubscriptionArn = true,
                Endpoint = queueUrl,
                Protocol = "sqs",
            };

            var response = await client.SubscribeAsync(request);
            return response;
        }

        /// <summary>
        /// Given the ARN for an Amazon SNS subscription, this method deletes
        /// the subscription.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to delete an Amazon SNS subscription.</param>
        /// <param name="subscriptionArn">The ARN of the subscription to delete.</param>
        /// <returns>Task.</returns>
        public static async Task UnsubscribeAsync(
            IAmazonSimpleNotificationService client,
            string subscriptionArn)
        {
            await client.UnsubscribeAsync(subscriptionArn);
        }

        /// <summary>
        /// Displays the list of Amazon SNS Topic ARNs.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to create an Amazon SNS subscription.</param>
        /// <returns>Task.</returns>
        public static async Task ListTopicsAsync(IAmazonSimpleNotificationService client)
        {
            Console.WriteLine($"Topics list\n --------------");

            var index = 1;
            var topicList = await GetTopicListAsync(client);

            foreach (var topic in topicList)
            {
                Console.WriteLine($"${index} --");
                Console.WriteLine($"{topic.TopicArn}");
                index++;
            }
        }

        private static async Task<List<Topic>> GetTopicListAsync(IAmazonSimpleNotificationService client)
        {
            var topics = new List<Topic>();
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