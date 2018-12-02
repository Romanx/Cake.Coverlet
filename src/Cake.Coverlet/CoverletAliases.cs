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
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }
            var currentCustomization = settings.ArgumentCustomization;
            settings.ArgumentCustomization = (args) => ArgumentsProcessor.ProcessMSBuildArguments(
                coverletSettings,
                context.Environment, 
                currentCustomization?.Invoke(args) ?? args, 
                project);

            context.DotNetCoreTest(project.FullPath, settings);
        }
        
        /// <summary>
        /// Runs coverlet with the given dll, test project and settings
        /// </summary>
        /// <param name="context"></param>
        /// <param name="testFile">The dll to instrument</param>
        /// <param name="testProject">The test project to run</param>
        /// <param name="settings">The coverlet settings to apply</param>
        [CakeMethodAlias]
        [CakeAliasCategory("Test")]
        public static void Coverlet(
            this ICakeContext context,
            FilePath testFile,
            FilePath testProject,
            CoverletSettings settings)
        {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (settings == null) {
                settings = new CoverletSettings();
            }

            new CoverletTool(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools)
                .Run(testFile, testProject, settings);
        }

        /// <summary>
        /// Runs the coverlet global tool with the given test project. The name of the 
        /// dll is inferred from the project name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="testProject">The test project to run</param>
        /// <param name="settings">The coverlet settings to apply</param>
        [CakeMethodAlias]
        [CakeAliasCategory("Test")]
        public static void Coverlet(
            this ICakeContext context,
            FilePath testProject,
            CoverletSettings settings)
        {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (settings == null) {
                settings = new CoverletSettings();
            }

            var debugFile = FindDebugDll(context, testProject.GetDirectory(), testProject);

            new CoverletTool(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools)
                .Run(debugFile, testProject, settings);
        }

        /// <summary>
        /// Runs the coverlet global tool with the given folder. We will discover any proj files, 
        /// take the first and infer the name of the dll from that.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="testProjectDir">The directory to find the dll and project from</param>
        /// <param name="settings">The coverlet settings to apply</param>
        [CakeMethodAlias]
        [CakeAliasCategory("Test")]
        public static void Coverlet(
            this ICakeContext context,
            DirectoryPath testProjectDir,
            CoverletSettings settings)
        {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (settings == null) {
                settings = new CoverletSettings();
            }
            
            var projFile = context.Globber.GetFiles($"{testProjectDir}/*.*proj")
                .FirstOrDefault();

            if (projFile == null)
            {
                throw new Exception($"Could not find valid proj file in {testProjectDir}");
            }

            var debugFile = FindDebugDll(context, testProjectDir, projFile);

            new CoverletTool(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools)
                .Run(debugFile, projFile, settings);
        }

        private static FilePath FindDebugDll(ICakeContext context, DirectoryPath path, FilePath filename)
        {
            var nameWithoutExtension = filename.GetFilenameWithoutExtension();
            var debugFile = context.Globber.GetFiles($"{path.MakeAbsolute(context.Environment)}/bin/**/Debug/**/{nameWithoutExtension}.dll")
                .FirstOrDefault();

            if (debugFile == null) 
            {
                throw new Exception($"Could not find debug dll with name {nameWithoutExtension}.dll");
            }

            return debugFile;
        }
    }
}
