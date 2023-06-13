using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApi.ServiceModel.Types;

public class Subscription
{
    public int UserId { get; set; }
    public int SessionId { get; set; }
    public string UserName { get; set; }
}