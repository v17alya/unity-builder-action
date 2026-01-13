using UnityEditor;

namespace Gamenator.Core.UnityBuilder.Core.Input
{
    /// <summary>
    /// Base class for all build option configurations.
    /// Contains common options shared across all platforms.
    /// </summary>
    public class OptionsBase
    {
        // ---------------------------------------------------------------------
        // Required Options
        // ---------------------------------------------------------------------

        /// <summary>
        /// Path to the Unity project directory.
        /// </summary>
        [Option("projectPath", true, 110, null)]
        public string ProjectPath { get; private set; }

        /// <summary>
        /// Unity build target (e.g., WebGL, Android, iOS).
        /// </summary>
        [Option("buildTarget", true, 120, BuildTarget.NoTarget)]
        public BuildTarget BuildTarget { get; private set; }

        /// <summary>
        /// Custom output path for the build (optional, defaults to platform-specific location).
        /// </summary>
        [Option("customBuildPath", true, 130, null)]
        public string CustomBuildPath { get; private set; }

        // ---------------------------------------------------------------------
        // Optional Options
        // ---------------------------------------------------------------------

        /// <summary>
        /// Build version string (e.g., "1.0.0"). Set to "none" to skip version update.
        /// </summary>
        [Option("buildVersion", false, 0, "none")]
        public string BuildVersion { get; private set; }

        /// <summary>
        /// Android version code (integer). Set to 0 to skip.
        /// </summary>
        [Option("androidVersionCode", false, 0, 0)]
        public int AndroidVersionCode { get; private set; }

        /// <summary>
        /// Whether this is a development build (enables debugging features).
        /// </summary>
        [Option("isDevelopmentBuild", false, 0, false)]
        public bool IsDevelopmentBuild { get; private set; }
    }
}
