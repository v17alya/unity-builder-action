using System;
using Gamenator.Core.UnityBuilder.Core;
using Gamenator.Core.UnityBuilder.Core.Reporting;
using Gamenator.Core.UnityBuilder.WebGL.Input;
using Gamenator.Core.UnityBuilder.WebGL.Reporting;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.WebGL;

namespace Gamenator.Core.UnityBuilder.WebGL
{
    /// <summary>
    /// WebGL platform-specific builder.
    /// Supports building for desktop and mobile WebGL with different texture compression formats.
    /// </summary>
    /// <remarks>
    /// This builder can perform two builds if <see cref="OptionsWebGL.IsMobile"/> is true:
    /// 1. Main build (DXT texture compression) - for desktop browsers
    /// 2. Mobile build (ASTC texture compression) - for mobile browsers
    /// </remarks>
    public class BuilderWebGL : Builder<StdInReporter, StdOutReporter, OptionsWebGL>
    {
        /// <summary>
        /// Gets the Unity build target (WebGL).
        /// </summary>
        protected override BuildTarget BuildTarget => BuildTarget.WebGL;

        /// <summary>
        /// Creates the input reporter for WebGL builds.
        /// </summary>
        /// <returns>An instance of <see cref="StdInReporter"/> with WebGL-specific options getter.</returns>
        protected override StdInReporter CreateStdInReporter() => new StdInReporter(new BuildOptionsGetterWebGL());

        /// <summary>
        /// Creates the output reporter for WebGL builds.
        /// </summary>
        /// <returns>An instance of <see cref="StdOutReporter"/> with WebGL-specific options getter.</returns>
        protected override StdOutReporter CreateStdOutReporter() => new StdOutReporter(new BuildOptionsGetterWebGL());

        /// <summary>
        /// Creates the parsed options instance for WebGL builds.
        /// </summary>
        /// <returns>An instance of <see cref="OptionsWebGL"/>.</returns>
        protected override OptionsWebGL CreateParsedOptions() => new OptionsWebGL();

        /// <summary>
        /// Implements the WebGL build logic.
        /// Performs main build (DXT) and optionally mobile build (ASTC) if IsMobile is true.
        /// </summary>
        /// <returns>The result of the build operation.</returns>
        protected override BuildResult BuildLogic()
        {
            BuildResult result;

#if UNITY_2021_2_OR_NEWER
            if (ParsedOptions.IsMobile)
            {
                // Build for both desktop (DXT) and mobile (ASTC) if mobile build is requested
                if ((result = MainBuild()) != BuildResult.Succeeded) return result;
                if ((result = MobileBuild()) != BuildResult.Succeeded) return result;
            }
            else
            {
                // Build only for desktop (DXT)
                if ((result = MainBuild()) != BuildResult.Succeeded) return result;
            }
#else
            // Unity versions before 2021.2 don't support subtargets, so only main build
            if ((result = MainBuild()) != BuildResult.Succeeded) return result;
#endif

            return result;

            /// <summary>
            /// Performs the main WebGL build with DXT texture compression (desktop browsers).
            /// </summary>
            /// <returns>The build result.</returns>
            BuildResult MainBuild()
            {
                Console.WriteLine($"{Utils.BuilderUtils.EOL}MainBuild(){Utils.BuilderUtils.EOL}");
                // Set texture compression to DXT for desktop browsers
                EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.DXT;
                var buildOptions = BaseBuildOptions;
                return Build(BuildTarget, buildOptions, (int)EditorUserBuildSettings.webGLBuildSubtarget, "build/WebGL/WebGL", default).result;
            }

            /// <summary>
            /// Performs the mobile WebGL build with ASTC texture compression (mobile browsers).
            /// </summary>
            /// <returns>The build result.</returns>
            BuildResult MobileBuild()
            {
                Console.WriteLine($"{Utils.BuilderUtils.EOL}MobileBuild(){Utils.BuilderUtils.EOL}");
                // Set texture compression to ASTC for mobile browsers
                EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.ASTC;
                var buildOptions = BaseBuildOptions;
                // Use "NOT_UPDATE_VERSION" define to prevent version update in second build
                return Build(BuildTarget, buildOptions, (int)EditorUserBuildSettings.webGLBuildSubtarget, "build/WebGL/MobileWebGL", new string[] { "NOT_UPDATE_VERSION" }).result;
            }
        }

        /// <summary>
        /// Applies WebGL-specific settings before building.
        /// Sets WASM code optimization level.
        /// </summary>
        protected override void ApplySettings()
        {
            base.ApplySettings();
            UserBuildSettings.codeOptimization = ParsedOptions.WasmCodeOptimization;
        }

        /// <summary>
        /// Gets the base build options for WebGL.
        /// </summary>
        /// <returns>The base build options.</returns>
        protected override BuildOptions GetBuildOptions()
        {
            var buildOptions = base.GetBuildOptions();
            // Uncomment the line below to enable auto-run player after build (for testing)
            // return buildOptions |= BuildOptions.AutoRunPlayer;
            return buildOptions;
        }
    }
}
