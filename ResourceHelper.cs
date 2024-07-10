using System;
using System.IO;
using System.Linq;
using System.Reflection;

public static class ResourceHelper
{
    public static string ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(resourceName));

        using (var stream = assembly.GetManifestResourceStream(resourcePath))
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}
