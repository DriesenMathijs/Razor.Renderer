using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Razor.Renderer.Core.Setup
{
    public class VirtualDirectoryContents : IDirectoryContents
    {
        public bool Exists => true;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            yield return Layout.Value;
            yield return MailTemplate.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Lazy<IFileInfo> MailTemplate { get; set; }

        public static Lazy<IFileInfo> Layout { get; } =
            new Lazy<IFileInfo>(() => new VirtualFileInfo("/Views/Shared/_Layout.cshtml",
                                                          "_Layout.cshtml",
                                                          (info) => Encoding.Default.GetBytes("<!DOCTYPE html> <html> <head> <meta name=\"viewport\" content=\"width = device - width\" /> <title>Layout from virtualFileProvider: @ViewBag.Title</title> </head> <body> <div> @RenderBody() </div> </body> </html>")));

    }
}
