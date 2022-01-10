using System.Reflection;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using EventViewer.Pipeline;
using McMaster.Extensions.CommandLineUtils;

namespace EventViewer {
    internal class Program
    {

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Argument(0, Description = @"Command.
        ListTopics -> find topics,
        Subscribe --topic-arn <arn> -> To add topic to listening list
        Unsubscribe --topic-arn <arn> -> To remove topic to listening list
        Listen to display incomming events"
        )]
        public string Command { get; }

        [Option("--topic-arn", Description = "Topic arn")]
        public string TopicArn { get; }

        [Option("--endpoint-url", Description = "Aws endpoint url")]
        public string EndpointUrl { get; } = "http://localhost:4566";

        private async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            var client = AmazonSimpleNotificationServiceClient(EndpointUrl);
            var queue = Queue.Create("DevListener");

            switch (Command)
            {
                case CommandOption.ListTopics:
                    await Topic.ListTopicsAsync(client);
                    break;
                case CommandOption.Subscribe:
                    await Topic.SubscribeAsync(client, topicArn, queueUrl);
                    break;
                case CommandOption.Unsubscribe:
                    var response = await Topic.SubscribeAsync(client, topicArn, queueUrl);
                    string subscriptionArn = response.SubscriptionArn;
                    await Topic.UnsubscribeAsync(client, subscriptionArn);
                    break;
                case CommandOption.Listen:
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

    public enum CommandOption
    {
        ListTopics,
        Subscribe,
        Listen,
        Unsubscribe,
        Help,
    }
}
