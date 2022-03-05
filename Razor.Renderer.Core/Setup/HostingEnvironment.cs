using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Razor.Renderer.Core.Setup
{
    internal class HostingEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = default;
        public string ApplicationName { get; set; } = default!;
        public string WebRootPath { get; set; } = default!;
        public IFileProvider WebRootFileProvider { get; set; } = default!;
        public string ContentRootPath { get; set; } = default!;
        public IFileProvider ContentRootFileProvider { get; set; } = default!;
    }
}
