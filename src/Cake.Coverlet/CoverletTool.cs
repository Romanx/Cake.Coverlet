using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Coverlet
{
    public sealed class CoverletTool : DotNetCoreTool<CoverletToolSettings>
    {
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cake.Common.Tools.DotNetCore.VSTest.DotNetCoreVSTester" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        public CoverletTool(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools)
          : base(fileSystem, environment, processRunner, tools)
        {
            this._environment = environment;
        }

        public void Run(IEnumerable<FilePath> testFiles, CoverletToolSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (testFiles == null || !testFiles.Any<FilePath>())
                throw new ArgumentNullException(nameof(testFiles));
            this.RunCommand(settings, this.GetArguments(testFiles, settings));
        }

        private ProcessArgumentBuilder GetArguments(IEnumerable<FilePath> testFiles, CoverletToolSettings settings)
        {
            var argumentBuilder = this.CreateArgumentBuilder(settings);
            
            throw new NotImplementedException();

            return argumentBuilder;
        }
    }

    public class CoverletToolSettings : DotNetCoreSettings
    {
        public CoverletSettings CoverletSettings { get; set; }
    }
}
