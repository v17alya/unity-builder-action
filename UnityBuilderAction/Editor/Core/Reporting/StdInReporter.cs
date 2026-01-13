using System;
using Gamenator.Core.UnityBuilder.Utils;
using UnityEditor;

namespace Gamenator.Core.UnityBuilder.Core.Reporting
{
    /// <summary>
    /// Reporter for logging build start information to standard input.
    /// Provides detailed information about build configuration before building.
    /// </summary>
    public class StdInReporter
    {
        /// <summary>
        /// Gets the build options getter used to format build information.
        /// </summary>
        private readonly BuildOptionsGetter _buildOptionsGetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="StdInReporter"/> class.
        /// </summary>
        /// <param name="buildOptionsGetter">The build options getter to use for formatting.</param>
        public StdInReporter(BuildOptionsGetter buildOptionsGetter)
        {
            _buildOptionsGetter = buildOptionsGetter;
        }

        /// <summary>
        /// Logs build start information including target, options, and player options.
        /// </summary>
        /// <param name="buildTarget">The Unity build target.</param>
        /// <param name="buildOptions">Build options flags.</param>
        /// <param name="buildPlayerOptions">Complete build player options.</param>
        public void LogBuildStart(BuildTarget buildTarget, BuildOptions buildOptions, BuildPlayerOptions buildPlayerOptions)
        {
            Console.WriteLine(
                            $"Start BuildMain{BuilderUtils.EOL}" +
                            $"###########################{BuilderUtils.EOL}" +
                            _buildOptionsGetter.GetBuildOptionsString(buildTarget, buildOptions) +
                            $"BuildPlayerOptions: {GetBuildPlayerOptionsString(buildPlayerOptions)}{BuilderUtils.EOL}"
                        );
        }

        /// <summary>
        /// Formats build player options as a string.
        /// </summary>
        /// <param name="buildPlayerOptions">The build player options to format.</param>
        /// <returns>Formatted string representation.</returns>
        private string GetBuildPlayerOptionsString(BuildPlayerOptions buildPlayerOptions)
        {
            return $"BuildPlayerOptions.scenes: {string.Join(", ", buildPlayerOptions.scenes)}{BuilderUtils.EOL}" +
                $"BuildPlayerOptions.locationPathName: {buildPlayerOptions.locationPathName}{BuilderUtils.EOL}" +
                $"BuildPlayerOptions.target: {buildPlayerOptions.target}{BuilderUtils.EOL}" +
                $"BuildPlayerOptions.options: {buildPlayerOptions.options}{BuilderUtils.EOL}" +
                $"BuildPlayerOptions.subtarget: {buildPlayerOptions.subtarget}{BuilderUtils.EOL}";
        }
    }
}
