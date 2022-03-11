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
            yield return TestFile.Value;
            yield return ModelFile.Value;
            yield return Text.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Lazy<IFileInfo> Text {get;set;}

        public static Lazy<IFileInfo> TestFile { get; } =
                    new Lazy<IFileInfo>(() => new VirtualFileInfo("custom:\\testapp\\test.cshtml",
                                                                  "test.cshtml",
                                                                  DateTimeOffset.Now,
                                                                  false,
                                                                  (info) => Encoding.Default.GetBytes("@(System.DateTime.Now)")));


        public static Lazy<IFileInfo> ModelFile { get; } =
            new Lazy<IFileInfo>(() => new VirtualFileInfo("custom:\\testapp\\model.cshtml",
                                                          "model.cshtml",
                                                          DateTimeOffset.Now,
                                                          false,
                                                          (info) => Encoding.Default.GetBytes(@"@model Razor.Renderer.Core.Virtual.TestModel
@foreach (var item in Model.Values)
{
<TEXT>@item
</TEXT>
}
")));

    }
}
