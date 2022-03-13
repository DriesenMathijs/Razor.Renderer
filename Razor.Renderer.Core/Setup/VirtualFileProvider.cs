using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Razor.Renderer.Core.Setup
{
    internal class VirtualFileProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string subpath) =>
            new VirtualDirectoryContents();

        public IFileInfo GetFileInfo(string subpath)
        {
            switch (subpath.ToLower())
            {
                case Constants.Layout:
                    return VirtualDirectoryContents.Layout.Value;
                case Constants.MailTemplate:
                    return VirtualDirectoryContents.MailTemplate.Value;
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
