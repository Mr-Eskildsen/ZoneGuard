using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Message.Control
{
    public enum ControlRequest
    {
        REQUEST_CONFIG = 1
    }

    public class MessageControl : MessageCore
    {
        [JsonProperty]
        public ControlRequest Request { get; private set; }

        public MessageControl() : base()
        {
        }

        public MessageControl(string senderId, ControlRequest request, DateTime timestamp) : base(senderId, timestamp)
        {
            Request = request;
        }

    }
}
