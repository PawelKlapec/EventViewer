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
        /// Help.
        /// </summary>
        Help,

        /// <summary>
        /// Subscriptions list.
        /// </summary>
        Subscriptions,

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
        Unsubscribe
    }

    /// <summary>
    /// Program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main Program.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>Integer.</returns>
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        /// <summary>
        /// Gets Command.
        /// </summary>
        [Argument(0, Description = "Command one of [ListTopics, Subscribe --topic-arn, Unsubscribe --topic-arn, Listen [Option: --pretty]")]
        #pragma warning disable SA1201 // Supress due to special decorator
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

        /// <summary>
        /// Gets a value indicating whether the item is enabled.
        /// </summary>
        [Option("--pretty", Description = "Pretty JSON view")]
        public bool Pretty { get; } = false;

        /// <summary>
        /// Gets All.
        /// </summary>
        [Option("--all", Description = "Param to get all")]
        public bool All { get; } = false;

        #pragma warning disable IDE0051 // Special method comming from CommandLineUtils
        #pragma warning disable IDE0060 // Special parameter comming from CommandLineUtils
        private async Task<int> OnExecuteAsync(
            CommandLineApplication app,
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
                    await PipelineTopic.PrintTopicListAsync(snsClient);
                    break;
                case CommandOption.Subscribe:
                    if(this.All){
                        await PipelineTopic.SubscribeAllQueueAsync(snsClient, queueUrl);
                        break;
                    }
                    await PipelineTopic.SubscribeQueueAsync(snsClient, this.TopicArn, queueUrl);
                    break;
                case CommandOption.Subscriptions:
                    await PipelineTopic.GetListSubscriptions(snsClient);
                    break;
                case CommandOption.Unsubscribe:
                    await PipelineTopic.UnsubscribeAsync(snsClient, this.TopicArn, queueUrl, this.All);
                    break;
                case CommandOption.Listen:
                    await PipelineQueue.ListenToSqsQueue(sqsClient, queueUrl, this.Pretty);
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
