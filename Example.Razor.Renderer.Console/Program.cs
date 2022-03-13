using Example.Razor.Renderer.Templates.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Razor.Renderer.Core.Logic;
using Razor.Renderer.Core.Logic.Interfaces;
using Razor.Renderer.Core.Setup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Razor.Renderer.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton<IRazorRenderEngine, RazorRenderEngine>();
            services.AddRazorRenderer();

            var serviceProvider = services.BuildServiceProvider();

            var razorRenderEngine = serviceProvider.GetRequiredService<IRazorRenderEngine>();

            var viewData = new Dictionary<string, object>();
            viewData.Add("ExampleVD", "ViewData");
            viewData.Add("ExampleVB", "ViewBag data");

            // Normally we await this method, but we're in the main here
            var renderedViewString = razorRenderEngine.RenderAsync("/Views/Example.cshtml", new ExampleModel() { Example = "Model" }, viewData).Result;

            System.Console.WriteLine($"Example with params:");
            System.Console.WriteLine($"--------------------");
            System.Console.WriteLine(renderedViewString);
            System.Console.WriteLine($"--------------------");
            System.Console.WriteLine($"--------------------");
            System.Console.WriteLine($"Example with content from code:");
            System.Console.WriteLine($"--------------------");

            // virtual file prov 
            VirtualDirectoryContents.MailTemplate = new Lazy<IFileInfo>(() => new VirtualFileInfo(
                                                              @"Views/templates/mailTemplate.cshtml",
                                                              "mailTemplate.cshtml",
                                                              (info) => Encoding.Default.GetBytes("@{ Layout = \"_Layout\";} @ViewBag.ExampleVB <br /> <br /> this is a mailtemplate with strings provided during runtime")));

            var rendered = razorRenderEngine.RenderAsync(@"Views/templates/mailtemplate.cshtml", viewData).Result;
            System.Console.WriteLine(rendered);

            System.Console.ReadLine();
        }
    }
}
