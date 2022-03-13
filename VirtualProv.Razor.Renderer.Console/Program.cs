using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Razor.Renderer.Core.Virtual;

namespace VirtualProv.Razor.Renderer.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var sp = ServiceProviderBuilder.BuildServiceProvider();
            var viewEngine = sp.GetRequiredService<IRazorViewEngine>();

            var razorRunner = sp.GetRequiredService<RazorRunner>();

            try
            {
                VirtualDirectoryContents.Template = new Lazy<IFileInfo>(() => new VirtualFileInfo(@"Views/templates/mailTemplate.cshtml",
                                                                  "mailTemplate.cshtml",
                                                                  (info) => Encoding.Default.GetBytes(@"<h1>rendered body</h1>")));

                var rendered = await razorRunner.Render(@"Views/templates/mailTemplate.cshtml");
                System.Console.WriteLine(rendered);

                //var rendered = await razorRunner.Render("custom:\\testapp\\test.cshtml");
                //System.Console.WriteLine(rendered);

                //rendered = await razorRunner.Render("custom:\\testapp\\model.cshtml", new TestModel { Values = new[] { "test", "model", "array", "stuff" } });
                //System.Console.WriteLine(rendered);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }

            System.Console.ReadLine();
        }
    }
}
