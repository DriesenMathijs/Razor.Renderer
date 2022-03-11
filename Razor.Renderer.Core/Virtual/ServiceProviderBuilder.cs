using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Razor.Renderer.Core.Virtual
{
    public static class ServiceProviderBuilder
    {
        public static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddLogging((builder) =>
            {
                builder.AddConsole(options =>
                {
                    options.IncludeScopes = true;
                });
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            services.AddRazor();

            services.AddSingleton<RazorRunner>();

            var result = services.BuildServiceProvider();
            return result;
        }

        public static IServiceCollection AddRazor(this IServiceCollection services)
        {
            var virtualFileProvider = new VirtualFileProvider();

            var hostingEnvironment = new CustomWebHostEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name
            };

            services.AddSingleton<IWebHostEnvironment>(hostingEnvironment);
            services.AddSingleton<DiagnosticSource>(new DiagnosticListener("DummySource"));
            services.AddSingleton<DiagnosticListener>(new DiagnosticListener("DummySource"));
            services.AddTransient<ObjectPoolProvider, DefaultObjectPoolProvider>();

            services.AddMvcCore()
                    .AddRazorViewEngine()
                    .AddRazorRuntimeCompilation(opt =>
                    {
                        opt.FileProviders.Clear();
                        opt.FileProviders.Add(virtualFileProvider);
                    });            

            return services;
        }
    }
}
