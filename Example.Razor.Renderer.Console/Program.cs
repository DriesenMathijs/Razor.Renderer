using Example.Razor.Renderer.Templates.Models;
using Microsoft.Extensions.DependencyInjection;
using Razor.Renderer.Core.Logic;
using Razor.Renderer.Core.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            viewData.Add("ExampleVB", "ViewBag");

            // Normally we await this method, but we're in the main here
            var renderedViewString = razorRenderEngine.RenderAsync("/Views/Example.cshtml", new ExampleModel() { Example = "Model" }, viewData).Result;

            System.Console.WriteLine($"Example with params:");
            System.Console.WriteLine($"--------------------");
            System.Console.WriteLine(renderedViewString);

            System.Console.ReadLine();
        }
    }
}
