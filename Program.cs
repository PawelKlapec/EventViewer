// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventViewer
{
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using EventViewer.Pipeline;
    using McMaster.Extensions.CommandLineUtils;

    /// <summary>
    /// CommandOption.
    /// </summary>
    public enum CommandOption
    {
        /// <summary>
        /// Lists topics.
        /// </summary>
        ListTopics,

        /// <summary>
        /// Subscribes topics to queue.
        /// </summary>
        Subscribe,

        /// <summary>
        /// Listens to queue.
        /// </summary>
        Listen,

        /// <summary>
        /// Unsubscribes topics to queue.
        /// </summary>
        Unsubscribe,

        /// <summary>
        /// Help.
        /// </summary>
        Help,
    }

    /// <summary>
    /// Program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main Program.
        /// </summary>
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        /// <summary>
        /// Gets Command.
        /// </summary>
        [Argument(0, Description = "Command one of [ListTopics, Subscribe --topic-arn, Unsubscribe --topic-arn, Listen")]
        public CommandOption Command { get; } = default!;

        /// <summary>
        /// Gets Topic Arn.
        /// </summary>
        [Option("--topic-arn", Description = "Topic arn")]
        public string TopicArn { get; } = default!;

        /// <summary>
        /// Gets EndpointUrl.
        /// </summary>
        [Option("--endpoint-url", Description = "Aws endpoint url")]
        public string EndpointUrl { get; } = "http://localhost:4566";

        private async Task<int> OnExecuteAsync(CommandLineApplication app,
                                               CancellationToken cancellationToken = default)
        {
            var snsClient = new AmazonSimpleNotificationServiceClient(
                new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = this.EndpointUrl,
                });
            var sqsClient = new AmazonSQSClient(
                new AmazonSQSConfig
                {
                    ServiceURL = this.EndpointUrl,
                });
            var queueUrl = await PipelineQueue.CreateAsync(sqsClient, "DevListener");

            switch (this.Command)
            {
                case CommandOption.ListTopics:
                    await PipelineTopic.ListTopicsAsync(snsClient);
                    break;
                case CommandOption.Subscribe:
                    await PipelineTopic.SubscribeQueueAsync(snsClient, this.TopicArn, queueUrl);
                    break;
                case CommandOption.Unsubscribe:
                    var response = await PipelineTopic.SubscribeQueueAsync(snsClient, this.TopicArn, queueUrl);
                    string subscriptionArn = response.SubscriptionArn;
                    await PipelineTopic.UnsubscribeAsync(snsClient, subscriptionArn);
                    break;
                case CommandOption.Listen:
                    await PipelineQueue.ListenToSqsQueue(sqsClient, queueUrl);
                    break;
                case CommandOption.Help:
                    app.ShowHelp();
                    break;
                default:
                    app.ShowHelp();
                    break;
            }

            return 0;
        }
    }
}
