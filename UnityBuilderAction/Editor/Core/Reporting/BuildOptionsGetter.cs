using Gamenator.Core.UnityBuilder.Utils;
using UnityEditor;

namespace Gamenator.Core.UnityBuilder.Core.Reporting
{
    /// <summary>
    /// Base class for formatting build options information for logging.
    /// Override this class to add platform-specific build option details.
    /// </summary>
    public class BuildOptionsGetter
    {
        /// <summary>
        /// Gets a formatted string representation of build options.
        /// </summary>
        /// <param name="buildTarget">The Unity build target.</param>
        /// <param name="buildOptions">Build options flags.</param>
        /// <returns>Formatted string containing build option information.</returns>
        public virtual string GetBuildOptionsString(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            return $"BuildOptions: {buildOptions}{BuilderUtils.EOL}";
        }
    }
}
