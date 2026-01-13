using System;
using System.IO;
using Gamenator.Core.UnityBuilder.Utils;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Gamenator.Core.UnityBuilder.Core.Reporting
{
    /// <summary>
    /// Reporter for logging build results and summary to standard output.
    /// Provides detailed information about build results and exits with appropriate codes.
    /// </summary>
    public class StdOutReporter
    {
        /// <summary>
        /// Gets the build options getter used to format build information.
        /// </summary>
        private readonly BuildOptionsGetter _buildOptionsGetter;

        /// <summary>
        /// Gets the end-of-line string for console output.
        /// </summary>
        protected static string EOL => BuilderUtils.EOL;

        /// <summary>
        /// Initializes a new instance of the <see cref="StdOutReporter"/> class.
        /// </summary>
        /// <param name="buildOptionsGetter">The build options getter to use for formatting.</param>
        public StdOutReporter(BuildOptionsGetter buildOptionsGetter)
        {
            _buildOptionsGetter = buildOptionsGetter;
        }

        /// <summary>
        /// Reports the build summary including duration, warnings, errors, and size.
        /// </summary>
        /// <param name="summary">The build summary to report.</param>
        public void ReportSummary(BuildSummary summary)
        {
            Console.WriteLine(
                $"{EOL}" +
                $"###########################{EOL}" +
                $"#      Build results      #{EOL}" +
                $"###########################{EOL}" +
                $"{EOL}" +
                $"Duration: {summary.totalTime.ToString()}{EOL}" +
                $"Warnings: {summary.totalWarnings.ToString()}{EOL}" +
                $"Errors: {summary.totalErrors.ToString()}{EOL}" +
                $"Size: {summary.totalSize.ToString()} bytes{EOL}" +
                _buildOptionsGetter.GetBuildOptionsString(summary.platform, summary.options) +
                $"{EOL}"
            );
        }

        /// <summary>
        /// Exits the Unity Editor with the appropriate exit code based on build result.
        /// </summary>
        /// <param name="result">The build result.</param>
        public static void ExitWithResult(BuildResult result)
        {
            Console.WriteLine("ExitWithResult!: " + result);
            switch (result)
            {
                case BuildResult.Succeeded:
                    Console.WriteLine("Build succeeded!");
                    EditorApplication.Exit(0);
                    break;
                case BuildResult.Failed:
                    Console.WriteLine("Build failed!");
                    EditorApplication.Exit(101);
                    break;
                case BuildResult.Cancelled:
                    Console.WriteLine("Build cancelled!");
                    EditorApplication.Exit(102);
                    break;
                case BuildResult.Unknown:
                default:
                    Console.WriteLine("Build result is unknown!");
                    EditorApplication.Exit(103);
                    break;
            }
        }

        /// <summary>
        /// Prints the catalog JSON file content to console (for Addressables support).
        /// </summary>
        /// <param name="report">The build report containing output path information.</param>
        public static void PrintCatalogJson(BuildReport report)
        {
            string buildPath = report.summary.outputPath;
            Console.WriteLine("PrintCatalogJson buildPath: " + buildPath);

            // Note: This is a simplified example. Adjust the path based on your Addressables setup.
            string relativeFilePath = "build/WebGL/WebGL/StreamingAssets/aa/catalog.json";
            Console.WriteLine("PrintCatalogJson relativeFilePath: " + relativeFilePath);
            PrintFile(relativeFilePath);
        }

        /// <summary>
        /// Prints the contents of a file to console if it exists.
        /// </summary>
        /// <param name="filePath">The path to the file to print.</param>
        private static void PrintFile(string filePath)
        {
            Console.WriteLine("PrintFile filePath: " + filePath);
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                Console.WriteLine("PrintFile: " + content);
            }
            else
            {
                Console.WriteLine("PrintFile file not found: " + filePath);
            }
        }
    }
}
