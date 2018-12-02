using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Coverlet
{
    /// <summary>
    /// A delegate representing the output transformation
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <param name="directoryPath">The directory path</param>
    /// <returns>The path and name of the file (without extension)</returns>
    public delegate string OutputTransformer(string fileName, string directoryPath);

    /// <summary>
    /// Settings used by Cake.Coverlet
    /// </summary>
    public class CoverletSettings : DotNetCoreSettings
    {
        /// <summary>
        /// Gets or sets if coverage should be collected
        /// </summary>
        public bool CollectCoverage { get; set; }

        /// <summary>
        /// Gets or sets the output format for Coverlet
        /// </summary>
        public CoverletOutputFormat CoverletOutputFormat { get; set; } = CoverletOutputFormat.json;

        /// <summary>
        /// Gets or sets the threshold for Coverlet to use in percent
        /// </summary>
        public uint? Threshold { get; set; }

        /// <summary>
        /// Gets or sets the type of threshold to apply.
        /// </summary>
        /// <remarks>
        /// This has no effect if Threshold is not set to a value
        /// </remarks>
        public ThresholdType ThresholdType { get; set; }

        /// <summary>
        /// Gets or sets the output directory the output files
        /// </summary>
        public DirectoryPath CoverletOutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the name of the output file excluding format
        /// </summary>
        public string CoverletOutputName { get; set; }

        /// <summary>
        /// Gets or sets the list of files to exclude
        /// </summary>
        public List<string> ExcludeByFile { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the exclusion filters
        /// </summary>
        public List<string> Exclude { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the file to merge the results of the run with
        /// </summary>
        public FilePath MergeWithFile { get; set; }

        /// <summary>
        /// Gets or sets a transformation function taking the <see cref="CoverletOutputName"/> and
        /// returning the new file name without an extension
        /// </summary>
        public OutputTransformer OutputTransformer { get; set; }
            = (fileName, dir) => $@"{dir}\{fileName}";

        /// <summary>
        /// Adds a filter to the list of exclusions
        /// </summary>
        /// <param name="filter">The filter to add</param>
        /// <returns></returns>
        public CoverletSettings WithFilter(string filter)
        {
            Exclude.Add(filter);
            return this;
        }

        /// <summary>
        /// Adds a file to the list of files to exclude
        /// </summary>
        /// <param name="file">The file to exclude</param>
        /// <returns></returns>
        public CoverletSettings WithFileExclusion(string file)
        {
            ExcludeByFile.Add(file);
            return this;
        }

        /// <summary>
        /// Add a type of threshold to combine with the existing
        /// </summary>
        /// <param name="type">The type of threshold to add</param>
        /// <returns></returns>
        public CoverletSettings WithThresholdType(ThresholdType type)
        {
            ThresholdType |= type;
            return this;
        }

        /// <summary>
        /// Add a type of format to combine with the existing output formats
        /// </summary>
        /// <param name="format">The format type to add</param>
        /// <returns></returns>
        public CoverletSettings WithFormat(CoverletOutputFormat format)
        {
            CoverletOutputFormat |= format;
            return this;
        }

        /// <summary>
        /// Add a default transformer appending the current date time at the time of calling test
        /// </summary>
        /// <returns></returns>
        public CoverletSettings WithDateTimeTransformer()
        {
            OutputTransformer = (fileName, directory) => $@"{directory}\{fileName}-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}";
            return this;
        }
        
        /// <summary>
        /// Sets the output format to be a specific value
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public CoverletSettings SetFormat(CoverletOutputFormat format)
        {
            CoverletOutputFormat = format;
            return this;
        }

        /// <summary>
        /// Clones the coverlet settings to a new instance
        /// </summary>
        /// <returns></returns>
        public CoverletSettings Clone()
        {
            return new CoverletSettings
            {
                CollectCoverage = CollectCoverage,
                CoverletOutputFormat = CoverletOutputFormat,
                Threshold = Threshold,
                ThresholdType = ThresholdType,
                CoverletOutputDirectory = CoverletOutputDirectory == null ? null : DirectoryPath.FromString(CoverletOutputDirectory.FullPath),
                CoverletOutputName = CoverletOutputName,
                ExcludeByFile = new List<string>(ExcludeByFile),
                Exclude = new List<string>(Exclude),
                MergeWithFile = MergeWithFile == null ? null : FilePath.FromString(MergeWithFile.FullPath),
                OutputTransformer = OutputTransformer
            };
        }
    }
}
