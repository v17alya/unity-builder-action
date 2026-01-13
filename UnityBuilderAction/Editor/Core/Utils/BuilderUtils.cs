using System;
using UnityEditor;
using UnityEditor.Build;

namespace Gamenator.Core.UnityBuilder.Utils
{
    /// <summary>
    /// Static utility class for Unity build operations.
    /// </summary>
    public static class BuilderUtils
    {
        /// <summary>
        /// Gets the end-of-line string for console output.
        /// </summary>
        public static string EOL = Environment.NewLine;

        /// <summary>
        /// Gets the named build target for a given build target.
        /// </summary>
        /// <param name="buildTarget">The Unity build target.</param>
        /// <returns>The named build target.</returns>
        public static NamedBuildTarget GetNamedBuildTarget(BuildTarget buildTarget)
           => NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(buildTarget));
    }
}
