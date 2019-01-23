using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Grpc.Core;
using Greet;
using Grpc.Core.Logging;

namespace Sample.Clients
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Environment.SetEnvironmentVariable("GRPC_TRACE", "api");
            //Environment.SetEnvironmentVariable("GRPC_VERBOSITY", "debug");
            //Grpc.Core.GrpcEnvironment.SetLogger(new ConsoleLogger());

            var channel = new Channel("localhost:50051", ClientResources.SslCredentials);
            var client = new Greeter.GreeterClient(channel);

            var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);

            var replies = client.SayHellos(new HelloRequest { Name = "GreeterClient" });
            while (await replies.ResponseStream.MoveNext(CancellationToken.None))
            {
                Console.WriteLine("Greeting: " + replies.ResponseStream.Current.Message);
            }

            Console.WriteLine("Shutting down");
            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
