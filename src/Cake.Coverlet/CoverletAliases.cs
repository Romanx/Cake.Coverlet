using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Cake.Coverlet
{
    /// <summary>
    /// Several extension methods when using DotNetCoreTest.
    /// </summary>
    [CakeAliasCategory("DotNetCore")]
    public static class CoverletAliases
    {
        /// <summary>
        /// Runs DotNetCoreTest using the given <see cref="CoverletSettings"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="project"></param>
        /// <param name="settings"></param>
        /// <param name="coverletSettings"></param>
        [CakeMethodAlias]
        [CakeAliasCategory("Test")]

        public static void DotNetCoreTest(
            this ICakeContext context,
            FilePath project,
            DotNetCoreTestSettings settings,
            CoverletSettings coverletSettings)
        {
            settings.ArgumentCustomization = args => ProcessArguments(context, args, project, coverletSettings);
            context.DotNetCoreTest(project.FullPath, settings);
        }

        private static ProcessArgumentBuilder ProcessArguments(
                ICakeContext cakeContext,
                ProcessArgumentBuilder builder,
                FilePath project,
                CoverletSettings settings)
        {
            builder.AppendProperty(nameof(CoverletSettings.CollectCoverage), settings.CollectCoverage.ToString());
            builder.AppendPropertyList(nameof(CoverletSettings.CoverletOutputFormat), SplitFlagEnum(settings.CoverletOutputFormat));

            if (settings.Threshold.HasValue)
            {
                if (settings.Threshold > 100)
                {
                    throw new Exception("Threshold Percentage cannot be set as greater than 100%");
                }

                builder.AppendProperty(nameof(CoverletSettings.Threshold), settings.Threshold.ToString());

                if (settings.ThresholdType != ThresholdType.NotSet)
                {
                    builder.AppendPropertyList(nameof(CoverletSettings.ThresholdType), SplitFlagEnum(settings.ThresholdType));
                }
            }

            if (!string.IsNullOrEmpty(settings.CoverletOutputName))
            {
                builder.AppendProperty(nameof(CoverletSettings.CoverletOutputName), settings.OutputNameTransformer(settings.CoverletOutputName));
            }

            if (settings.CoverletOutputDirectory != null)
            {
                builder.AppendProperty(nameof(CoverletSettings.CoverletOutputDirectory), settings.CoverletOutputDirectory.MakeAbsolute(cakeContext.Environment).FullPath);
            }

            if (settings.ExcludeByFile.Count > 0)
            {
                builder.AppendPropertyList(nameof(CoverletSettings.ExcludeByFile), settings.ExcludeByFile);
            }

            if (settings.Exclude.Count > 0)
            {
                builder.AppendPropertyList(nameof(CoverletSettings.Exclude), settings.Exclude);
            }

            return builder;

            IEnumerable<string> SplitFlagEnum(Enum @enum) => @enum.ToString().Split(',').Select(s => s.ToLowerInvariant());
        }
    }
}
