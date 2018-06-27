using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Coverlet
{
    internal static class BuilderExtension
    {
        internal static ProcessArgumentBuilder AppendProperty(this ProcessArgumentBuilder builder, string propertyName, string value)
        {
            builder.Append($"/property:{propertyName}={value}");
            return builder;
        }

        internal static ProcessArgumentBuilder AppendPropertyList(this ProcessArgumentBuilder builder, string propertyName, IEnumerable<string> values)
        {
            builder.Append($"/property:{propertyName}=\\\"{string.Join(",", values.Select(s => s.Trim()))}\\\"");
            return builder;
        }
    }
}
