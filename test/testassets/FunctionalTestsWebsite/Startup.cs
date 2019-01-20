using FunctionalTestsWebsite.Infrastructure;
using Grpc.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionalTestsWebsite
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<IncrementingCounter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Workaround for https://github.com/aspnet/AspNetCore/issues/6880
            app.Use((context, next) =>
            {
                if (!context.Response.SupportsTrailers())
                {
                    context.Features.Set<IHttpResponseTrailersFeature>(new TestHttpResponseTrailersFeature
                    {
                        Trailers = new HttpResponseTrailers()
                    });
                }

                return next();
            });

            app.UseEndpointRouting(builder =>
            {
                builder.MapGrpcService<ChatterService>();
                builder.MapGrpcService<CounterService>();
                builder.MapGrpcService<GreeterService>();
            });
        }
    }
}
