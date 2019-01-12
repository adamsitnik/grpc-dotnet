using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace Grpc.AspNetCore
{
    internal class HttpContextStreamReader<TRequest> : IAsyncStreamReader<TRequest>
    {
        private HttpContext _httpContext;
        private Func<byte[], TRequest> _deserializer;

        public HttpContextStreamReader(HttpContext context, Func<byte[], TRequest> deserializer)
        {
            _httpContext = context;
            _deserializer = deserializer;
        }

        public TRequest Current { get; private set; }


        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            var requestPayload = await StreamUtils.ReadMessageAsync(_httpContext.Request.Body);

            if (requestPayload == null)
            {
                Current = default(TRequest);
                return false;
            }

            Current = _deserializer(requestPayload);
            return true;
        }
    }
}
