using System.Collections.Generic;
using MyApi.ServiceModel.Types;
using ServiceStack;

namespace MyApi.ServiceModel;

[Route("/channels/general/chat")]
public class PostChatToGeneral : IReturn<ChatMessage>
{
    public string From { get; set; }
    public string ToUserId { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public string Selector { get; set; }
}

[Route("/channels/general/raw")]
public class PostRawToGeneral : IReturnVoid
{
    public string From { get; set; }
    public string ToUserId { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public string Selector { get; set; }
}


[Route("/channels/casino/chat")]
public class PostChatToCasino : IReturn<ChatMessage>
{
    public string From { get; set; }
    public string ToUserId { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public string Selector { get; set; }
}

[Route("/channels/casino/raw")]
public class PostRawToCasino : IReturnVoid
{
    public string From { get; set; }
    public string ToUserId { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public string Selector { get; set; }
}

[Route("/chathistory")]
public class GetChatHistory : IReturn<GetChatHistoryResponse>
{
    public string[] Channels { get; set; }
    public long? AfterId { get; set; }
    public int? Take { get; set; }
}

public class GetChatHistoryResponse
{
    public List<ChatMessage> Results { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}


//[Route("/get-subscriptions")]
//public class GetSubscriptions : IReturn<GetSubscriptionsResponse>
//{
//    public string? UserId { get; set; } = string.Empty;
//    public int? SessionId { get; set; }
//    public string? UserName { get; set; } = string.Empty;
//}

//public class GetSubscriptionsResponse :
//{
//    public List<SubscriptionInfo> Results { get; set; }
//    public ResponseStatus ResponseStatus { get; set; }
//}
