using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.AspNetCore
{
    internal class ClientStreamingServerCallHandler<TRequest, TResponse, TImplementation> : IServerCallHandler
        where TRequest : IMessage
        where TResponse : IMessage
        where TImplementation : class
    {
        private MessageParser _inputParser;
        private string _methodName;

        public ClientStreamingServerCallHandler(MessageParser inputParser, string methodName)
        {
            _methodName = methodName;
            _inputParser = inputParser;
        }

        public async Task HandleCallAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/grpc";
            httpContext.Response.Headers.Append("grpc-encoding", "identity");

            // Activate the implementation type via DI.
            var activator = httpContext.RequestServices.GetRequiredService<IGrpcServiceActivator<TImplementation>>();
            var service = activator.Create();

            // Select procedure using reflection
            var handlerMethod = typeof(TImplementation).GetMethod(_methodName);

            // Invoke procedure
            var response = await (Task<TResponse>)handlerMethod.Invoke(
                service,
                new object[]
                {
                    new HttpContextStreamReader<TRequest>(httpContext, bytes => (TRequest)_inputParser.ParseFrom(bytes)),
                    null
                });

            // TODO: make sure the response is not null
            var responsePayload = response.ToByteArray();

            await StreamUtils.WriteMessageAsync(httpContext.Response.Body, responsePayload, 0, responsePayload.Length);

            httpContext.Response.AppendTrailer("grpc-status", ((int)StatusCode.OK).ToString());
        }
    }
}
