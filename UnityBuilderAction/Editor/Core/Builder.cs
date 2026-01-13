using System;
using System.Linq;
using Gamenator.Core.UnityBuilder.Core.Input;
using Gamenator.Core.UnityBuilder.Core.Reporting;
using Gamenator.Core.UnityBuilder.Utils;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Gamenator.Core.UnityBuilder.Core
{
    /// <summary>
    /// Abstract base class for Unity build automation.
    /// Provides a template method pattern for implementing platform-specific builders.
    /// </summary>
    /// <typeparam name="T1">Type of the input reporter (receives build start information).</typeparam>
    /// <typeparam name="T2">Type of the output reporter (reports build results).</typeparam>
    /// <typeparam name="T3">Type of the parsed options class (platform-specific build options).</typeparam>
    public abstract class Builder<T1, T2, T3>
        where T1 : StdInReporter
        where T2 : StdOutReporter
        where T3 : OptionsBase
    {
        /// <summary>
        /// Reporter for logging build start information.
        /// </summary>
        protected readonly T1 StdInReporter;

        /// <summary>
        /// Reporter for logging build results and summary.
        /// </summary>
        protected readonly T2 StdOutReporter;

        /// <summary>
        /// Parsed command-line options for this build.
        /// </summary>
        protected readonly T3 ParsedOptions;

        /// <summary>
        /// Default scripting define symbols for the build target.
        /// </summary>
        protected readonly string[] DefaultDefines;

        /// <summary>
        /// Base build options (can be modified by subclasses).
        /// </summary>
        protected BuildOptions BaseBuildOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Builder{T1, T2, T3}"/> class.
        /// Parses command-line arguments and sets up reporters.
        /// </summary>
        public Builder()
        {
            StdInReporter = CreateStdInReporter();
            StdOutReporter = CreateStdOutReporter();
            ParsedOptions = CreateParsedOptions();
            new OptionsParser(Environment.GetCommandLineArgs())
                .ParseInto(ParsedOptions);

            // Gather default scripting define symbols
            DefaultDefines = PlayerSettings.GetScriptingDefineSymbols(BuilderUtils.GetNamedBuildTarget(BuildTarget)).Split(';');
            BaseBuildOptions = GetBuildOptions();
        }

        /// <summary>
        /// Gets the Unity build target for this builder.
        /// </summary>
        protected abstract BuildTarget BuildTarget { get; }

        /// <summary>
        /// Creates an instance of the input reporter.
        /// </summary>
        /// <returns>An instance of the input reporter.</returns>
        protected abstract T1 CreateStdInReporter();

        /// <summary>
        /// Creates an instance of the output reporter.
        /// </summary>
        /// <returns>An instance of the output reporter.</returns>
        protected abstract T2 CreateStdOutReporter();

        /// <summary>
        /// Creates an instance of the parsed options class.
        /// </summary>
        /// <returns>An instance of the parsed options class.</returns>
        protected abstract T3 CreateParsedOptions();

        /// <summary>
        /// Implements the platform-specific build logic.
        /// </summary>
        /// <returns>The result of the build operation.</returns>
        protected abstract BuildResult BuildLogic();

        /// <summary>
        /// Executes the build process.
        /// Applies settings, runs build logic, and exits with the appropriate result code.
        /// </summary>
        public void Build()
        {
            ApplySettings();
            StdOutReporter.ExitWithResult(BuildLogic());
        }

        /// <summary>
        /// Performs a Unity build with the specified parameters.
        /// </summary>
        /// <param name="buildTarget">The Unity build target.</param>
        /// <param name="options">Build options flags.</param>
        /// <param name="buildSubtarget">Platform-specific subtarget (e.g., texture format for WebGL).</param>
        /// <param name="filePath">Output path for the build.</param>
        /// <param name="extraScriptingDefines">Additional scripting define symbols to add for this build.</param>
        /// <returns>The build summary containing results and statistics.</returns>
        protected BuildSummary Build(BuildTarget buildTarget, BuildOptions options, int buildSubtarget, string filePath, string[] extraScriptingDefines = default)
        {
            string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(s => s.path).ToArray();
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                target = buildTarget,
                locationPathName = filePath,
                // targetGroup = BuildTargetGroup.WebGL,
                options = options,
                // extraScriptingDefines = extraScriptingDefines, // for some reasons this doesn't work, so use PlayerSettings.SetScriptingDefineSymbolsForGroup isntead
#if UNITY_2021_2_OR_NEWER
                subtarget = buildSubtarget
#endif
            };
            
            StdInReporter.LogBuildStart(buildTarget, options, buildPlayerOptions);

            if (extraScriptingDefines != default)
                PlayerSettings.SetScriptingDefineSymbols(BuilderUtils.GetNamedBuildTarget(buildTarget), string.Join(";", DefaultDefines.Concat(extraScriptingDefines)));

            var definesText = string.Join(";", UnityEditor.PlayerSettings.GetScriptingDefineSymbols(BuilderUtils.GetNamedBuildTarget(buildTarget)));
            // Console.WriteLine($"Extra defines: {definesText}");

            BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary buildSummary = buildReport.summary;
            StdOutReporter.ReportSummary(buildSummary);
            StdOutReporter.PrintCatalogJson(buildReport);
            if (buildSummary.result != BuildResult.Succeeded) StdOutReporter.ExitWithResult(buildSummary.result);
            return buildSummary;
        }

        /// <summary>
        /// Applies build settings before building.
        /// Sets version, Android version code, and development build flag.
        /// Override this method to add platform-specific settings.
        /// </summary>
        protected virtual void ApplySettings()
        {
            // Set version for this build
            if (ParsedOptions.BuildVersion is not null and not "none")
            {
                Console.WriteLine($"{BuilderUtils.EOL}buildVersion: " + ParsedOptions.BuildVersion);
                PlayerSettings.bundleVersion = ParsedOptions.BuildVersion;
                PlayerSettings.macOS.buildNumber = ParsedOptions.BuildVersion;
            }
            if (ParsedOptions.AndroidVersionCode != 0)
            {
                Console.WriteLine($"{BuilderUtils.EOL}androidVersionCode: " + ParsedOptions.AndroidVersionCode);
                PlayerSettings.Android.bundleVersionCode = ParsedOptions.AndroidVersionCode;
            }

            BaseBuildOptions = BaseBuildOptions.AddIf(BuildOptions.Development, ParsedOptions.IsDevelopmentBuild);
            Console.WriteLine($"{BuilderUtils.EOL}IsDevelopmentBuild: " + ParsedOptions.IsDevelopmentBuild);
        }

        /// <summary>
        /// Gets the base build options.
        /// Override this method to add platform-specific build options.
        /// </summary>
        /// <returns>The base build options.</returns>
        protected virtual BuildOptions GetBuildOptions() => BuildOptions.None;
    }
}
