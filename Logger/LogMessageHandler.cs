using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus.Logging;

namespace Logger
{
    public class LogMessageHandler
        :
        IHandleMessages<LogMessage>
    {
        static ILog log = LogManager.GetLogger<LogMessageHandler>();

        public Task Handle(LogMessage message, IMessageHandlerContext context)
        {
            log.Info("Received Message");
            log.Info($"Message Id: {message.MessageId}");
            log.Info($"Message Content: {message.MessageContent}");
            log.Info($"Message Channel: {message.MessageChannel}");
            log.Info($"From User Id: {message.FromUserId}");
            

            return Task.CompletedTask;
        }
    }
}
