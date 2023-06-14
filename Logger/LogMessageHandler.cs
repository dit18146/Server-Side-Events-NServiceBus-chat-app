using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus.Logging;

namespace Logger
{
    class LogMessageHandler : Saga<LogMessageHandler.LogPolicyData>,
        IAmStartedByMessages<LogMessage>

    {
        static ILog log = LogManager.GetLogger<LogMessageHandler>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<LogPolicyData> mapper)
        {
          

            mapper.MapSaga(sagaData => sagaData.MessageId)
                .ToMessage<LogMessage>(message => message.MessageId);
        }

        
        public async Task Handle(LogMessage message, IMessageHandlerContext context)
        {
            if (!Data.IsMessageAnnouncedSent)
            {
                log.Info("Received Message");
                log.Info($"Message Id: {message.MessageId}");
                log.Info($"Message Content: {message.MessageContent}");
                log.Info($"Message Channel: {message.MessageChannel}");
                log.Info($"Message Channel: {message.MessageSelector}");
                log.Info($"From User Id: {message.FromUserId}");

                var messageAnounced = new MessageAnounced()
                {
                    MessageId = message.MessageId,
                    MessageContent = "Message Received successfully by the endpoint",
                    MessageSelector = message.MessageSelector,
                    MessageChannel = message.MessageChannel,
                    FromUserId = message.FromUserId,
                };

                Data.IsMessageAnnouncedSent = true;
                await context.Publish(messageAnounced).ConfigureAwait(false);
            }
            else
            {
                Data.IsMessageAnnouncedSent = false;
            }
        }


        internal class LogPolicyData : ContainSagaData
        {
            public string MessageId { get; set; } = string.Empty;
            public bool IsReplyReceived {get; set; }

            public bool IsMessageAnnouncedSent{ get; set; }
        }
    }

  
}
