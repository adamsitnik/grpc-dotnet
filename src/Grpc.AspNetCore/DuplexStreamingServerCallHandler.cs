using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.AspNetCore
{
    internal class DuplexStreamingServerCallHandler<TRequest, TResponse, TImplementation> : IServerCallHandler
        where TRequest : IMessage
        where TResponse : IMessage
        where TImplementation : class
    {
        private MessageParser _inputParser;
        private string _methodName;

        public DuplexStreamingServerCallHandler(MessageParser inputParser, string methodName)
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
            await (Task)handlerMethod.Invoke(
                service,
                new object[] {
                    new HttpContextStreamReader<TRequest>(httpContext, bytes => (TRequest)_inputParser.ParseFrom(bytes)),
                    new HttpContextStreamWriter<TResponse>(httpContext, response => response.ToByteArray()),
                    null
                });

            httpContext.Response.AppendTrailer("grpc-status", ((int)StatusCode.OK).ToString());
        }
    }
}
