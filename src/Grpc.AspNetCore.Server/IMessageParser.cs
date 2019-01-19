using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.AspNetCore
{
    public interface IMessageParser
    {
        // TODO: Read message without allocating array
        object ParseFrom(byte[] data);
    }
}
