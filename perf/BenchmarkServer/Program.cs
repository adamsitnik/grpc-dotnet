using System;
using System.IO;
using System.Net;
using Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;

namespace PerformanceBenchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureKestrel((context, options) =>
                {
                    options.Limits.MinRequestBodyDataRate = null;

                    var endPoint = context.Configuration.CreateIPEndPoint();

                    options.Listen(endPoint, listenOptions =>
                    {
                        if (Convert.ToBoolean(context.Configuration["UseTls"]))
                        {
                            listenOptions.UseHttps(Resources.ServerPFXPath, "1111");
                        }

                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                })
                .UseStartup<Startup>();
    }
}
