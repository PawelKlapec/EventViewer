// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventViewer
{
    using EventViewer.Aws;
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
        Unsubscribe,
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
        [Argument(0, Description = "Command one of [ListTopics, Subscriptions, Subscribe [--topic-arn or --all], Unsubscribe [--topic-arn or --all], Listen [Option: --pretty]")]
        #pragma warning disable SA1201 // Supress due to special decorator
        public CommandOption Command { get; } = CommandOption.Help;

        /// <summary>
        /// Gets Topic Arn.
        /// </summary>
        [Option("--topic-arn", Description = "Topic arn")]
        public string TopicArn { get; } = Environment.GetEnvironmentVariable("TOPIC_ARN") ?? string.Empty;

        /// <summary>
        /// Gets EndpointUrl.
        /// </summary>
        [Option("--endpoint-url", Description = "Aws endpoint url")]
        public string EndpointUrl { get; } = Environment.GetEnvironmentVariable("ENDPOINT_URL") ?? "localhost:4566";

        /// <summary>
        /// Gets a value indicating whether the item is enabled.
        /// </summary>
        [Option("--pretty", Description = "Pretty JSON view")]
        public bool Pretty { get; } = bool.Parse(Environment.GetEnvironmentVariable("PRETTY") ?? "false");

        /// <summary>
        /// Gets a value indicating whether all.
        /// </summary>
        [Option("--all", Description = "Param to get all")]
        public bool All { get; } = false;

        #pragma warning disable IDE0051 // Special method comming from CommandLineUtils
        #pragma warning disable IDE0060 // Special parameter comming from CommandLineUtils
        private async Task<int> OnExecuteAsync(
            CommandLineApplication app,
            CancellationToken cancellationToken = default)
        {   
            var client = new Aws.Client(this.EndpointUrl);
            var queueUrl = await PipelineQueue.CreateAsync(sqsClient, "DevListener");

            switch (this.Command)
            {
                case CommandOption.ListTopics:
                    await PipelineTopic.PrintTopicListAsync(client.Sns);
                    break;
                case CommandOption.Subscribe:
                    if (this.All)
                    {
                        await PipelineTopic.SubscribeAllQueueAsync(client.Sns, queueUrl);
                        break;
                    }

                    await PipelineTopic.SubscribeQueueAsync(client.Sns, this.TopicArn, queueUrl);
                    break;
                case CommandOption.Subscriptions:
                    await PipelineTopic.GetListSubscriptions(client.Sns, cancellationToken);
                    break;
                case CommandOption.Unsubscribe:
                    await PipelineTopic.UnsubscribeAsync(client.Sns, this.TopicArn, queueUrl, this.All, cancellationToken);
                    break;
                case CommandOption.Listen:
                    await PipelineQueue.ListenToSqsQueue(client.Sqs, queueUrl, this.Pretty);
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
