// <copyright file="Client.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventViewer.Aws
{
    using Amazon.Runtime;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;

    /// <summary>
    /// Setting up AWS clients.
    /// </summary>
    public class Client
    {
        private readonly AmazonSimpleNotificationServiceClient snsClient;
        private readonly AmazonSQSClient sqsClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="endpointUrl"> AWS endpoint.
        public Client(string endpointUrl)
        {
            this.snsClient = SetSnsClient(endpointUrl);
            this.sqsClient = SetSqsClient(endpointUrl);
        }

        /// <summary>
        /// Gets Sns client.
        /// </summary>
        public AmazonSimpleNotificationServiceClient Sns
        {
            get { return this.snsClient; }
        }

        /// <summary>
        /// Gets Sqs client.
        /// </summary>
        public AmazonSQSClient Sqs
        {
            get { return this.sqsClient; }
        }

        /// <summary>
        /// This method returns Sns Client.
        /// </summary>
        /// <param name="endpointUrl"> AWS endpoint.
        /// <returns>AmazonSimpleNotificationServiceClient.</returns>
        private static AmazonSimpleNotificationServiceClient SetSnsClient(string endpointUrl)
        {
            var credentials = new BasicAWSCredentials("key", "secret");
            var config = new AmazonSimpleNotificationServiceConfig
            {
            ServiceURL = endpointUrl,
            };

            return new AmazonSimpleNotificationServiceClient(credentials, config);
        }

        /// <summary>
        /// This method returns Sqs Client.
        /// </summary>
        /// <param name="endpointUrl"> AWS endpoint.
        /// <returns>AmazonSQSClient.</returns>
        private static AmazonSQSClient SetSqsClient(string endpointUrl)
        {
            var credentials = new BasicAWSCredentials("key", "secret");
            var config = new AmazonSQSConfig
            {
            ServiceURL = endpointUrl,
            };

            return new AmazonSQSClient(credentials, config);
        }
    }
}