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

                case "/custom:/testapp/test.cshtml":
                    return VirtualDirectoryContents.TestFile.Value;
                case "/custom:/testapp/model.cshtml":
                    return VirtualDirectoryContents.ModelFile.Value;
                case "/custom:/testappss/testss.cshtml":
                    return VirtualDirectoryContents.Text.Value;
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
