using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Razor.Renderer.Core.Virtual
{
    public class VirtualFileProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string subpath) =>
            new VirtualDirectoryContents();

        public IFileInfo GetFileInfo(string subpath)
        {
            switch (subpath.ToLower())
            {               
                case "/views/_viewstart.cshtml":
                    return VirtualDirectoryContents.ViewStart.Value;
                case "/views/shared/_layout.cshtml":
                    return VirtualDirectoryContents.Layout.Value;
                case "/views/templates/mailtemplate.cshtml":
                    return VirtualDirectoryContents.Template.Value;
                default:
                    return new NotFoundFileInfo(subpath);
            }
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}
