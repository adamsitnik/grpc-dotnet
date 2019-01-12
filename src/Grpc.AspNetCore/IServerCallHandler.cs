
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Grpc.AspNetCore
{
    interface IServerCallHandler
    {
        Task HandleCallAsync(HttpContext httpContext);
    }
}
