using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz.Azure.ServiceBus.Library
{
    public interface IMessageProperties
    {
        Guid Id { get; set; }
    }
}
