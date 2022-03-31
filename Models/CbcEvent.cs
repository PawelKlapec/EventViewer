// <copyright file="CbcEvent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventViewer.Models
{
    using System;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// CbcEvent.
    /// </summary>
    [JsonObject]
    public class CbcEvent
    {
        /// <summary>
        /// Gets or sets Command.
        /// </summary>
        [JsonProperty("Type")]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Gets or sets MessageId.
        /// </summary>
        [JsonProperty("MessageId")]
        public string MessageId { get; set; } = default!;

        /// <summary>
        /// Gets or sets TopicArn.
        /// </summary>
        [JsonProperty("TopicArn")]
        public string TopicArn { get; set; } = default!;

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        [JsonProperty("Message")]
        public string Message { get; set; } = default!;

        /// <summary>
        /// Converts tp string.
        /// </summary>
        /// <param name="pretty">Makes result prettier.</param>
        /// <returns>String.</returns>
        public string ToString(bool pretty)
        {
            var returnMessage = pretty ? JToken.Parse(this.Message).ToString() : this.Message;

            StringBuilder sb = new ();
            sb.AppendLine($"-- Begin: {this.MessageId} at {DateTime.Now} -------- ");
            sb.AppendLine(string.Empty);
            sb.AppendLine($"  MessageId: {this.MessageId}");
            sb.AppendLine($"  Type: {this.Type}");
            sb.AppendLine($"  TopicArn: {this.TopicArn}");
            sb.AppendLine($"  Message: {returnMessage}");
            sb.AppendLine(string.Empty);
            sb.AppendLine($"-- End: {this.MessageId} ----------------------------------- ");
            return sb.ToString();
        }

        /// <summary>
        /// Converts tp string.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            return this.ToString(false);
        }
    }
}