using Razor.Renderer.Core.Logic;
using Razor.Renderer.Core.Logic.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRazorRenderer(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var razorRendererEngine = serviceProvider.GetRequiredService<IRazorRenderEngine>();

            RazorRendererLogicFactory.ServiceCollection = services;
            razorRendererEngine.Setup();
        }
    }
}
