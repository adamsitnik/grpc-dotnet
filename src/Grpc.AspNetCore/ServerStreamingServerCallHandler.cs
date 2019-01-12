using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using GRPCServer.Dotnet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.AspNetCore
{
    internal class ServerStreamingServerCallHandler<TRequest, TResponse, TImplementation> : IServerCallHandler
        where TRequest : IMessage
        where TResponse : IMessage
        where TImplementation : class
    {
        private string _methodName;
        private MessageParser _inputParser;

        public ServerStreamingServerCallHandler(MessageParser inputParser, string methodName)
        {
            _methodName = methodName;
            _inputParser = inputParser;
        }

        public async Task HandleCallAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/grpc";
            httpContext.Response.Headers.Append("grpc-encoding", "identity");

            var requestPayload = await StreamUtils.ReadMessageAsync(httpContext.Request.Body);
            // TODO: make sure the payload is not null
            var request = (TRequest)_inputParser.ParseFrom(requestPayload);

            // TODO: make sure there are no more request messages.

            // Activate the implementation type via DI.
            var activator = httpContext.RequestServices.GetRequiredService<IGrpcServiceActivator<TImplementation>>();
            var service = activator.Create();

            // Select procedure using reflection
            var handlerMethod = typeof(TImplementation).GetMethod(_methodName);

            // Invoke procedure
            await (Task)handlerMethod.Invoke(
                service,
                new object[]
                {
                    request,
                    new HttpContextStreamWriter<TResponse>(httpContext, response => response.ToByteArray()),
                    null
                });

            httpContext.Response.AppendTrailer("grpc-status", ((int)StatusCode.OK).ToString());
        }
    }
}
