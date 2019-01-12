using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace Grpc.AspNetCore
{
    internal class HttpContextStreamWriter<TResponse> : IServerStreamWriter<TResponse>
    {
        HttpContext _httpContext;
        Func<TResponse, byte[]> _serializer;

        public HttpContextStreamWriter(HttpContext context, Func<TResponse, byte[]> serializer)
        {
            _httpContext = context;
            _serializer = serializer;
        }

        public WriteOptions WriteOptions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task WriteAsync(TResponse message)
        {
            // TODO: make sure the response is not null
            var responsePayload = _serializer(message);
            return StreamUtils.WriteMessageAsync(_httpContext.Response.Body, responsePayload, 0, responsePayload.Length);
        }
    }
}
