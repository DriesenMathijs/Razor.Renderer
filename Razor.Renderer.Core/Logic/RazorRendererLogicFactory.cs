using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Razor.Renderer.Core.Setup;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Razor.Renderer.Core.Logic
{
    internal class RazorRendererLogicFactory
    {
        private static IServiceCollection _serviceCollection;

        public static IServiceCollection ServiceCollection
        {
            get
            {
                if (_serviceCollection is null)
                    return new ServiceCollection();

                return _serviceCollection;
            }
            set
            {
                _serviceCollection = value;
            }
        }

        /// <summary>
        /// Create the instance of IServiceScopeFactory
        /// </summary>
        /// <returns></returns>
        public IServiceScopeFactory CreateRendererServicesScopeFactory()
        {
            var services = ServiceCollection;

            // Setup some variables we need to make the razor engine work
            var assembliesBaseDirectory = AppContext.BaseDirectory;
            var mainExecutableDirectory = GetMainExecutableDirectory();
            var webRootDirectory = GetWebRootDirectory(assembliesBaseDirectory);
            var fileProvider = new PhysicalFileProvider(assembliesBaseDirectory);
            
            // Initiate the VirtualFileProvider to render templates during runtime
            var virtualFileProvider = new VirtualFileProvider();

            services.TryAddSingleton<IWebHostEnvironment>(new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? Constants.Identifier,
                ContentRootPath = assembliesBaseDirectory,
                ContentRootFileProvider = fileProvider,
                WebRootPath = webRootDirectory,
                WebRootFileProvider = new PhysicalFileProvider(webRootDirectory)
            });
            services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.TryAddSingleton<DiagnosticSource>(new DiagnosticListener(Constants.Identifier));
            services.TryAddSingleton<DiagnosticListener>(new DiagnosticListener(Constants.Identifier));
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddLogging();
            services.AddHttpContextAccessor();

            // Add Mvc (for Razor-mvc syntax usage), RazorViewEngine (to render the views; both physical and runtime compiled)
            // Add the VirtualFileProvider to fetch virtual files (runtime provided strings)
            var builder = services.AddMvcCore()
                                  .AddRazorViewEngine()
                                  .AddRazorRuntimeCompilation(opt =>
                                  {
                                      opt.FileProviders.Clear();
                                      opt.FileProviders.Add(virtualFileProvider);
                                  });

            // Load view assembly application parts to find the view from shared libraries
            builder.ConfigureApplicationPartManager(manager =>
            {
                var parts = ApplicationPartsHelper.GetApplicationParts();

                foreach (var part in parts)
                {
                    // For MVC projects, application parts are already added by the framework
                    if (!manager.ApplicationParts.Any(x => x.Name == part.Name))
                        manager.ApplicationParts.Add(part);

                }
            });

            // so views in a Razor Class Library are included
            services.Configure<MvcRazorRuntimeCompilationOptions>(o =>
            {
                o.FileProviders.Add(fileProvider);
            });

            services.TryAddTransient<RazorRendererLogic>();

            return services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
        }

        /// <summary>
        /// Returns the path of the main executable file using which the application is started
        /// </summary>
        /// <returns></returns>
        private string GetMainExecutableDirectory()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }

        /// <summary>
        /// Get the web root directory where the static content resides. This is to add support for MVC applications
        /// If the webroot directory doesn't exist, set the path to assembly base directory.
        /// </summary>
        /// <param name="assembliesBaseDirectory"></param>
        /// <returns></returns>
        private string GetWebRootDirectory(string assembliesBaseDirectory)
        {
            var webRootDirectory = Path.Combine(assembliesBaseDirectory, "wwwroot");
            if (!Directory.Exists(webRootDirectory))
            {
                webRootDirectory = assembliesBaseDirectory;
            }

            return webRootDirectory;
        }
    }
}
