using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FunctionalTestsWebsite.Infrastructure
{
    public class TestHttpResponseTrailersFeature : IHttpResponseTrailersFeature
    {
        public IHeaderDictionary Trailers { get; set; }
    }
}
