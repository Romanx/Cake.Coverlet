using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cake.Common.IO.Paths;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;
using Cake.Testing.Fixtures;

namespace Cake.Coverlet.Tests;

public record CoverletToolResult(FilePath ToolPath, Path Path, Dictionary<string, IReadOnlyList<string>> Arguments)
{
    public static CoverletToolResult Convert(ToolFixtureResult result)
    {
        var strPath = GetValue((QuotedArgument)result.Process.Arguments.First());
        var filePath = FilePath.FromString(strPath);

        Path finalPath = filePath.HasExtension
            ? filePath
            : DirectoryPath.FromString(strPath);
        
        var arguments = result.Process.Arguments.Skip(1)
            .Select(arg =>
            {
                if (arg is SwitchArgument sa)
                {
                    return ConvertSwitch(sa);
                }

                throw new InvalidOperationException("Not a switch argument");
            })
            .GroupBy(x => x.Switch)
            .ToDictionary<IGrouping<string, (string Switch, string Value)>, string, IReadOnlyList<string>>(k => k.Key, v =>
            {
                return [..v.Select(kvp => kvp.Value)];
            });
        
        return new CoverletToolResult(result.Path, finalPath, arguments);

        static (string Switch, string Value) ConvertSwitch(SwitchArgument arg)
        {
            var name = GetSwitchName(arg);
            var argument = GetArgument(arg) switch
            {
                QuotedArgument qa => GetValue(qa),
                TextArgument ta => GetText(ta),
                _ => throw new InvalidOperationException($"Unable to convert argument type {GetArgument(arg).GetType().FullName}")
            };
            
            return (name, argument);
        
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_switch")]
            static extern ref string GetSwitchName(SwitchArgument @this);
        
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_argument")]
            static extern ref IProcessArgument GetArgument(SwitchArgument @this);
        }
        
        static string GetValue(QuotedArgument qa)
        {
            var inner = GetArgument(qa);
            if (inner is TextArgument ta)
                return GetText(ta);

            throw new InvalidOperationException($"Unable to convert argument type {inner.GetType().FullName}");
            
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_argument")]
            static extern ref IProcessArgument GetArgument(QuotedArgument @this);
        }
        
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_text")]
        static extern ref string GetText(TextArgument @this);
    }
}
