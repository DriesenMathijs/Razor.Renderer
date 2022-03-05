using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace Razor.Renderer.Core.Setup
{
    internal class CustomRouter : IRouter
    {
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            return Task.CompletedTask;
        }
    }
}
