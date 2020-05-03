using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz.Azure.ServiceBus.Library.Tests.Models
{
    public class DummyModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Message { get; set; } = "Dummy";

        public DateTime Stamp { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"Id: {this.Id}, Message: {this.Message}, Stamp: {this.Stamp}";
        }
    }
}
