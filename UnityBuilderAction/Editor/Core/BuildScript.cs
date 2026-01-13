using System;
using Gamenator.Core.UnityBuilder.Core.Exceptions;
using Gamenator.Core.UnityBuilder.Core.Input;
using Gamenator.Core.UnityBuilder.Core.Reporting;
using Gamenator.Core.UnityBuilder.WebGL;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Gamenator.Core.UnityBuilder
{
    /// <summary>
    /// Main entry point for Unity build automation.
    /// This class is called by GameCI Unity Builder action via buildMethod parameter.
    /// </summary>
    /// <remarks>
    /// Usage in GitHub Actions:
    /// <code>
    /// buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
    /// </code>
    /// </remarks>
    public static class BuildScript
    {
        /// <summary>
        /// Main build entry point called by Unity command-line build.
        /// Parses command-line arguments and delegates to the appropriate platform-specific builder.
        /// </summary>
        /// <remarks>
        /// To add support for a new platform:
        /// 1. Create a new builder class (e.g., BuilderAndroid) inheriting from Builder&lt;T1, T2, T3&gt;
        /// 2. Create platform-specific options class (e.g., OptionsAndroid) inheriting from OptionsBase
        /// 3. Add a case in the switch statement below
        /// 4. See README.md for detailed extension instructions
        /// </remarks>
        public static void BuildMain()
        {
            try
            {
                var optionsParser = new OptionsParser(Environment.GetCommandLineArgs());
                var optionsBase = new OptionsBase();

                // Determine which builder to use based on buildTarget argument
                var builder = optionsParser.ParseProperty(optionsBase, o => o.BuildTarget) switch
                {
                    BuildTarget.WebGL => new BuilderWebGL(),
                    // TODO: Add more platforms here
                    // BuildTarget.Android => new BuilderAndroid(),
                    // BuildTarget.iOS => new BuilderIOS(),
                    // BuildTarget.StandaloneWindows64 => new BuilderWindows(),
                    _ => null
                };

                if (builder == null)
                    throw new Exception($"Unsupported build target: {optionsBase.BuildTarget}. See README.md for extension instructions.");

                builder.Build();
            }
            catch (ArgumentMissingException argumentMissingException)
            {
                Console.WriteLine("BuildMain() argumentMissingException: " + argumentMissingException.Message);
                EditorApplication.Exit(argumentMissingException.ErrorCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("BuildMain() exception: " + ex.Message);
                StdOutReporter.ExitWithResult(BuildResult.Failed);
            }
        }
    }
}
