using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ServiceStack;
using MyApi.ServiceModel;
using MyApi.ServiceModel.Types;
using ServiceStack.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Messages;
using MyApi.ServiceModel.Types;
using MyApi.ServiceModel;
using ServiceStack.Messaging;
using NServiceBus;
using NServiceBus.Logging;
using Endpoint = NServiceBus.Endpoint;
using ServiceStack.Web;
using System.Net.Http;
using System.Threading.Channels;
using static System.Net.WebRequestMethods;

namespace MyApi.ServiceInterface;


public class ServerEventsServices : Service
{
    public IServerEvents ServerEvents { get; set; }
    public IChatHistory ChatHistory { get; set; }
    public IAppSettings AppSettings { get; set; }
    public IEventSubscription Subscription { get; set; }

    private IMessageSession _bus;

    public ServerEventsServices(IMessageSession bus)
    {
        _bus = bus;
    }

    public async void Any(PostRawToGeneral request)
    {
        if (!IsAuthenticated && AppSettings.Get("LimitRemoteControlToAuthenticatedUsers", false))
            throw new HttpError(HttpStatusCode.Forbidden, "You must be authenticated to use remote control.");

        // Ensure the subscription sending this notification is still active
        var sub = ServerEvents.GetSubscriptionInfo(request.From);

        if (sub == null)
            throw HttpError.NotFound($"Subscription {request.From} does not exist");

        // Check to see if this is a private message to a specific user
        if (request.ToUserId != null)
        {
            // Only notify that specific user
            await ServerEvents.NotifyUserIdAsync(request.ToUserId, request.Selector, request.Message).ConfigureAwait(false);
        }
        else
        {
            // Notify everyone in the channel for public messages
            //await ServerEvents.NotifyChannelAsync(request.Channel, request.Selector, request.Message).ConfigureAwait(false);

            // Notify everyone 
            await ServerEvents.NotifyAllAsync(request.Selector, request.Message).ConfigureAwait(false);

            //Notify NServiceBus
            await  SendCommand(request.Message, request.Channel, request.Selector,request.From).ConfigureAwait(false);
        }
    }

    public object Any(PostChatToGeneral request)
    {
        Console.WriteLine("innnnnnnnnnn");
        // Ensure the subscription sending this notification is still active
        var sub = ServerEvents.GetSubscriptionInfo(request.From);
        if (sub == null)
            throw HttpError.NotFound("Subscription {0} does not exist".Fmt(request.From));

        var channel = request.Channel;

        try
        {
            // Create a DTO ChatMessage to hold all required info about this message
            var msg = new ChatMessage
            {
                Id = DateTime.UtcNow.Ticks,
                Channel = request.Channel,
                FromUserId = sub.UserId,
                FromName = sub.DisplayName,
                Message = request.Message,
            };

            // Check to see if this is a private message to a specific user
            if (request.ToUserId != null)
            {
                // Mark the message as private so it can be displayed differently in Chat
                msg.Private = true;
                // Send the message to the specific user Id
                ServerEvents.NotifyUserId(request.ToUserId, request.Selector, msg);

                // Also provide UI feedback to the user sending the private message so they
                // can see what was sent. Relay it to all senders active subscriptions 
                var toSubs = ServerEvents.GetSubscriptionInfosByUserId(request.ToUserId);

                foreach (var toSub in toSubs)
                {
                    // Change the message format to contain who the private message was sent to
                    msg.Message = $"@{toSub.DisplayName}: {msg.Message}";
                    ServerEvents.NotifySubscription(request.From, request.Selector, msg);
                }
            }
            else
            {
                // Notify everyone in the channel for public messages
                ServerEvents.NotifyChannel(request.Channel, request.Selector, msg);

                //Notify NServiceBus
                SendCommand(request.Message, request.Channel, request.Selector,request.From).ConfigureAwait(false);
            }

            if (!msg.Private)
                ChatHistory.Log(channel, msg);

            return msg;
        }
        catch (Exception ex)
        {
            // create an HTTP exception with a more informative message
            throw new HttpError(System.Net.HttpStatusCode.InternalServerError,
                $"Failed to add message. id: {sub.UserId}, channel: {request.Channel}, from name: {sub.DisplayName}, to userId: {request.ToUserId}, message: {request.Message} and selector:{request.Selector}   request: {request}.  Error: {ex.Message}, line: {ex.StackTrace}");
        }
    }

     public async void Any(PostRawToCasino request)
    {
        if (!IsAuthenticated && AppSettings.Get("LimitRemoteControlToAuthenticatedUsers", false))
            throw new HttpError(HttpStatusCode.Forbidden, "You must be authenticated to use remote control.");

        // Ensure the subscription sending this notification is still active
        var sub = ServerEvents.GetSubscriptionInfo(request.From);

        if (sub == null)
            throw HttpError.NotFound($"Subscription {request.From} does not exist");

        // Check to see if this is a private message to a specific user
        if (request.ToUserId != null)
        {
            // Only notify that specific user
            await ServerEvents.NotifyUserIdAsync(request.ToUserId, request.Selector, request.Message).ConfigureAwait(false);
        }
        else
        {
            // Notify everyone in the channel for public messages
            await ServerEvents.NotifyChannelAsync(request.Channel, request.Selector, request.Message).ConfigureAwait(false);

            //Notify NServiceBus
            await  SendCommand(request.Message, request.Channel, request.Selector,request.From).ConfigureAwait(false);
        }
    }

    public object Any(PostChatToCasino request)
    {
        // Ensure the subscription sending this notification is still active
        var sub = ServerEvents.GetSubscriptionInfo(request.From);
        if (sub == null)
            throw HttpError.NotFound("Subscription {0} does not exist".Fmt(request.From));

        var channel = request.Channel;

        try
        {
            // Create a DTO ChatMessage to hold all required info about this message
            var msg = new ChatMessage
            {
                Id = DateTime.UtcNow.Ticks,
                Channel = request.Channel,
                FromUserId = sub.UserId,
                FromName = sub.DisplayName,
                Message = request.Message,
            };

            // Check to see if this is a private message to a specific user
            if (request.ToUserId != null)
            {
                // Mark the message as private so it can be displayed differently in Chat
                msg.Private = true;
                // Send the message to the specific user Id
                ServerEvents.NotifyUserId(request.ToUserId, request.Selector, msg);

                // Also provide UI feedback to the user sending the private message so they
                // can see what was sent. Relay it to all senders active subscriptions 
                var toSubs = ServerEvents.GetSubscriptionInfosByUserId(request.ToUserId);

                foreach (var toSub in toSubs)
                {
                    // Change the message format to contain who the private message was sent to
                    msg.Message = $"@{toSub.DisplayName}: {msg.Message}";
                    ServerEvents.NotifySubscription(request.From, request.Selector, msg);
                }
            }
            else
            {
                // Notify everyone in the channel for public messages
                ServerEvents.NotifyChannel(request.Channel, request.Selector, msg);

                //Notify NServiceBus
                SendCommand(request.Message, request.Channel, request.Selector,request.From).ConfigureAwait(false);
            }

            if (!msg.Private)
               ChatHistory.Log(channel, msg);

            return msg;
        }
        catch (Exception ex)
        {
            // create an HTTP exception with a more informative message
            throw new HttpError(System.Net.HttpStatusCode.InternalServerError,
                $"Failed to add message. id: {sub.UserId}, channel: {request.Channel}, from name: {sub.DisplayName}, to userId: {request.ToUserId}, message: {request.Message} and selector:{request.Selector}   request: {request}. Error: {ex.Message}");
        }
    }

    public async Task SendCommand(string content, string channel, string selector, string fromUserId)
    {
        var command = new LogMessage()
        {
            MessageId = fromUserId,
            MessageContent = content,
            MessageChannel = channel,
            MessageSelector = selector,
            FromUserId = fromUserId,
        };

        await _bus.Send(command)
            .ConfigureAwait(false);
    }

    //public object Any(GetSubscriptions request)
    //{
    //    return new GetSubscriptionsResponse
    //    {
    //        Results = ServerEvents.GetAllSubscriptionInfos().ConvertAll(SubscriptionInfo => Subscription);
    //    };
    //}

    public object Any(GetChatHistory request)
    {
        var msgs = request.Channels.Map(x =>
                ChatHistory.GetRecentChatHistory(x, request.AfterId, request.Take))
            .SelectMany(x => x)
            .OrderBy(x => x.Id)
            .ToList();

        return new GetChatHistoryResponse
        {
            Results = msgs
        };
    }
}
public interface IChatHistory
{
    long GetNextMessageId(string channel);

    void Log(string channel, ChatMessage msg);

    List<ChatMessage> GetRecentChatHistory(string channel, long? afterId, int? take);

    void Flush();
}

public class MemoryChatHistory : IChatHistory
{
    public int DefaultLimit { get; set; }

    public IServerEvents ServerEvents { get; set; }

    public MemoryChatHistory()
    {
        DefaultLimit = 100;
    }

    Dictionary<string, List<ChatMessage>> MessagesMap = new Dictionary<string, List<ChatMessage>>();

    public long GetNextMessageId(string channel)
    {
        return ServerEvents.GetNextSequence("chatMsg");
    }

    public void Log(string channel, ChatMessage msg)
    {
        if (!MessagesMap.TryGetValue(channel, out var msgs))
            MessagesMap[channel] = msgs = new List<ChatMessage>();

        msgs.Add(msg);
    }

    public List<ChatMessage> GetRecentChatHistory(string channel, long? afterId, int? take)
    {
        if (!MessagesMap.TryGetValue(channel, out var msgs))
            return new List<ChatMessage>();

        var ret = msgs.Where(x => x.Id > afterId.GetValueOrDefault())
            .Reverse()  //get latest logs
            .Take(take.GetValueOrDefault(DefaultLimit))
            .Reverse(); //reverse back

        return ret.ToList();
    }

    public void Flush()
    {
        MessagesMap = new Dictionary<string, List<ChatMessage>>();
    }
}

public class MessageAnouncedHandler : IHandleMessages<MessageAnounced>
{
    

    public async Task Handle(MessageAnounced message, IMessageHandlerContext context)
    {
        var baseUri = "https://localhost:5001";
        var channel = "general";

        //var client = new ServerEventsClient(baseUri, channel);

        //// Send a Web Service Request using the built-in JsonServiceClient
        //client.ServiceClient.Post(new PostChatToGeneral()
        //{
        //    Channel = message.MessageChannel,     // The channel we're listening on
        //    From = message.FromUserId, 
        //    Message = message.MessageContent,
        //    Selector = message.MessageSelector
        //});

        var client = new JsonApiClient(baseUri);

        // Send a Web Service Request using the built-in JsonServiceClient
        await client.PostAsync(new PostChatToGeneral()
        {
            Channel = message.MessageChannel,     // The channel we're listening on
            From = message.FromUserId, 
            Message = message.MessageContent,
            Selector = message.MessageSelector
        }).ConfigureAwait(false);


    }
}

