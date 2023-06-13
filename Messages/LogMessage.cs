using System.Windows.Input;
using ICommand = NServiceBus.ICommand;


namespace Messages
{
    public class LogMessage :
        ICommand
    {
        public string MessageId { get; set; } = string.Empty;

        public string MessageContent { get; set; } = string.Empty;

        public string MessageChannel { get; set; } = string.Empty;

        public string FromUserId { get; set;} = string.Empty;
    }
}