using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneGuard.Shared.Message
{
    public abstract class MessageConfig : MessageCore
    {
        protected MessageConfig(string id, DateTime timestamp) : base(id, timestamp)
        {
        }

    }
}
