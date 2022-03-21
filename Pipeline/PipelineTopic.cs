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
        /// Creates a new subscription to a topic.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to create an Amazon SNS subscription.</param>
        /// <param name="queueUrl">The queue url.</param>
        /// <returns>A SubscribeResponse object which includes the subscription
        /// ARN for the new subscription.</returns>
        public static async Task<Dictionary<string, Task<SubscribeResponse>>> SubscribeAllQueueAsync(
            IAmazonSimpleNotificationService client,
            string queueUrl)
        {
            var responseList = new Dictionary<string, Task<SubscribeResponse>>();
            var topicList = await PipelineTopic.GetTopicListAsync(client);
            foreach (var topic in topicList)
            {
                var response = SubscribeQueueAsync(client, topic.TopicArn, queueUrl);
                responseList.Add(topic.TopicArn, response);
            }

            return responseList;
        }

        /// <summary>
        /// Given the ARN for an Amazon SNS subscription, this method deletes
        /// the subscription.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to delete an Amazon SNS subscription.</param>
        /// <param name="topicArn">The ARN of the topic to delete.</param>
        /// <param name="queueUrl">The queue url.</param>
        /// <param name="all">The all flag.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>Task.</returns>
        public static async Task UnsubscribeAsync(
            IAmazonSimpleNotificationService client,
            string topicArn,
            string queueUrl,
            bool all,
            CancellationToken cancellationToken)
        {
            if (!all)
            {
                var response = await SubscribeQueueAsync(client, topicArn, queueUrl);
                string subscriptionArn = response.SubscriptionArn;
                await client.UnsubscribeAsync(subscriptionArn, cancellationToken);
                return;
            }

            var listSubscriptions = await PipelineTopic.GetListSubscriptions(client, cancellationToken);
            foreach (var item in listSubscriptions)
            {
                await client.UnsubscribeAsync(item.SubscriptionArn, cancellationToken);
            }
        }

        /// <summary>
        /// Displays the list of Amazon SNS Topic ARNs.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to create an Amazon SNS subscription.</param>
        /// <returns>Task.</returns>
        public static async Task PrintTopicListAsync(IAmazonSimpleNotificationService client)
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

        /// <summary>
        /// Return the list of Amazon SNS Topic ARNs.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to create an Amazon SNS subscription.</param>
        /// <returns>Task with List of Topic.</returns>
        public static async Task<List<Topic>> GetTopicListAsync(IAmazonSimpleNotificationService client)
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

        /// <summary>
        /// Returns a list of the requester's subscriptions.
        /// </summary>
        /// <param name="client">The initialized Amazon SNS client object, used
        /// to create an Amazon SNS subscription.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>Task with List of Topic.</returns>
        public static async Task<List<Subscription>> GetListSubscriptions(IAmazonSimpleNotificationService client, CancellationToken cancellationToken)
        {
            var response = await client.ListSubscriptionsAsync(cancellationToken);
            Console.WriteLine($"Subscriptions list [{response.Subscriptions.Count}]\n--------------");
            var index = 1;
            foreach (var item in response.Subscriptions)
            {
                Console.WriteLine($"{index}.   {item.TopicArn}");
                index++;
            }

            return response.Subscriptions;
        }
    }
}