using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApi.ServiceModel.Types;
public class ChatMessage
{
    public long Id { get; set; }

    public string Channel { get; set; }

    public string FromUserId { get; set; }

    public string FromName { get; set; }

    public string Message { get; set; }

    public bool Private { get; set; }
}