using Gamenator.Core.UnityBuilder.Core.Reporting;
using Gamenator.Core.UnityBuilder.Utils;
using UnityEditor;
using UnityEditor.WebGL;

namespace Gamenator.Core.UnityBuilder.WebGL.Reporting
{
    /// <summary>
    /// WebGL-specific build options getter.
    /// Provides detailed WebGL build configuration information for logging.
    /// </summary>
    public class BuildOptionsGetterWebGL : BuildOptionsGetter
    {
        /// <summary>
        /// Gets a formatted string representation of WebGL build options.
        /// Includes IL2CPP settings, code optimization, and texture compression format.
        /// </summary>
        /// <param name="buildTarget">The Unity build target.</param>
        /// <param name="buildOptions">Build options flags.</param>
        /// <returns>Formatted string containing WebGL-specific build option information.</returns>
        public override string GetBuildOptionsString(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            return base.GetBuildOptionsString(buildTarget, buildOptions) +
                $"PlayerSettings.GetIl2CppCodeGeneration: {PlayerSettings.GetIl2CppCodeGeneration(BuilderUtils.GetNamedBuildTarget(buildTarget))}{BuilderUtils.EOL}" +
                $"PlayerSettings.GetIl2CppCompilerConfiguration: {PlayerSettings.GetIl2CppCompilerConfiguration(BuilderUtils.GetNamedBuildTarget(buildTarget))}{BuilderUtils.EOL}" +
                $"EditorUserBuildSettings.GetPlatformSettings(\"CodeOptimization\"): {EditorUserBuildSettings.GetPlatformSettings(BuildPipeline.GetBuildTargetName(buildTarget), "CodeOptimization")}{BuilderUtils.EOL}" +
                $"UserBuildSettings.CodeOptimization: {UserBuildSettings.codeOptimization}{BuilderUtils.EOL}" +
                $"EditorUserBuildSettings.webGLBuildSubtarget: {EditorUserBuildSettings.webGLBuildSubtarget}{BuilderUtils.EOL}";
        }
    }
}
