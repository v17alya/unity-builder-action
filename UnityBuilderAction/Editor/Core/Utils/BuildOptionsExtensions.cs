using UnityEditor;

namespace Gamenator.Core.UnityBuilder.Utils
{
    /// <summary>
    /// Extension methods for <see cref="BuildOptions"/>.
    /// </summary>
    public static class BuildOptionsExtensions
    {
        /// <summary>
        /// Adds <paramref name="flag"/> to <paramref name="opts"/> if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="opts">The build options to modify.</param>
        /// <param name="flag">The build option flag to add.</param>
        /// <param name="condition">Whether to add the flag.</param>
        /// <returns>The modified build options.</returns>
        public static BuildOptions AddIf(this BuildOptions opts, BuildOptions flag, bool condition)
            => condition ? opts | flag : opts;
    }
}
