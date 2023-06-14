using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class MessageAnounced :
        IEvent
    {
        public string MessageId { get; set; } = string.Empty;

        public string MessageContent { get; set; } = string.Empty;

        public string MessageChannel { get; set; } = string.Empty;

        public string MessageSelector { get; set; } = string.Empty;

        public string FromUserId { get; set;} = string.Empty;
    }
}
