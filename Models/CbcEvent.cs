namespace EventViewer.Models
{
    using System;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [JsonObject]
    public class CbcEvent {

        [JsonProperty("Type")]
        public string Type { get; set; } = default!;

        [JsonProperty("MessageId")]
        public string MessageId { get; set; } = default!;

        [JsonProperty("TopicArn")]
        public string TopicArn { get; set; } = default!;

        [JsonProperty("Message")]
        public string Message { get; set; } = default!;

        public string ToString(bool pretty)
        {
            var returnMessage = pretty ? JToken.Parse(Message).ToString() : Message;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"-- Begin: {MessageId} at {DateTime.Now} -------- ");
            sb.AppendLine(string.Empty);
            sb.AppendLine($"  MessageId: {MessageId}");
            sb.AppendLine($"  Type: {Type}");
            sb.AppendLine($"  TopicArn: {TopicArn}");
            sb.AppendLine($"  Message: {returnMessage}");
            sb.AppendLine(string.Empty);
            sb.AppendLine($"-- End: {MessageId} ----------------------------------- ");
            return sb.ToString();
        }

         public override string ToString()
        {
            return ToString(false);
        }
    }
}