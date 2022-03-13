using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Razor.Renderer.Core.Virtual
{
    public class VirtualDirectoryContents : IDirectoryContents
    {
        public bool Exists => true;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            yield return Layout.Value;
            yield return ViewStart.Value;
            yield return Template.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Lazy<IFileInfo> Template {get;set;}

        public static Lazy<IFileInfo> ViewStart { get; } =
                    new Lazy<IFileInfo>(() => new VirtualFileInfo("/Views/_ViewStart.cshtml",
                                                                  "_ViewStart.cshtml",
                                                                  (info) => Encoding.Default.GetBytes("@{ Layout = \"_Layout\"; }")));

        public static Lazy<IFileInfo> Layout { get; } =
            new Lazy<IFileInfo>(() => new VirtualFileInfo("/Views/Shared/_Layout.cshtml",
                                                          "_Layout.cshtml",
                                                          (info) => Encoding.Default.GetBytes("<!DOCTYPE html> <html> <head> <meta name=\"viewport\" content=\"width = device - width\" /> <title>@ViewBag.Title</title> </head> <body> <div> @RenderBody() </div> </body> </html>")));

    }
}
