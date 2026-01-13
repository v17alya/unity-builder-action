using Gamenator.Core.UnityBuilder.Core.Input;
using UnityEditor.WebGL;

namespace Gamenator.Core.UnityBuilder.WebGL.Input
{
    /// <summary>
    /// WebGL-specific build options.
    /// Extends <see cref="OptionsBase"/> with WebGL-specific settings.
    /// </summary>
    public class OptionsWebGL : OptionsBase
    {
        /// <summary>
        /// Default WASM code optimization level.
        /// </summary>
        private const WasmCodeOptimization DEFAULT_WASM_CODE_OPTIMIZATION = WasmCodeOptimization.RuntimeSpeed;

        // ---------------------------------------------------------------------
        // Required Options
        // ---------------------------------------------------------------------

        /// <summary>
        /// WASM code optimization level.
        /// RuntimeSpeed: Optimized for runtime performance (larger file size).
        /// Size: Optimized for file size (slower runtime performance).
        /// </summary>
        [Option("codeOptimization", true, 140, DEFAULT_WASM_CODE_OPTIMIZATION)]
        public WasmCodeOptimization WasmCodeOptimization { get; private set; }

        // ---------------------------------------------------------------------
        // Optional Options
        // ---------------------------------------------------------------------

        /// <summary>
        /// Whether to build for mobile WebGL (ASTC texture compression).
        /// If true, performs two builds: desktop (DXT) and mobile (ASTC).
        /// Only supported in Unity 2021.2 or newer.
        /// </summary>
        [Option("isMobile", false, 0, false)]
        public bool IsMobile { get; private set; }
    }
}
