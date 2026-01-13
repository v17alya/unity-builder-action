# Unity Builder Action

Customizable Unity build automation system for [GameCI GitHub Actions](https://game.ci/docs/github/builder). Provides a flexible, extensible framework for building Unity projects across multiple platforms with platform-specific configurations.

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Usage](#-usage)
- [Adding New Arguments](#-adding-new-arguments)
- [Architecture](#-architecture)
- [Extending to New Platforms](#-extending-to-new-platforms)
- [GitHub Actions Integration](#-github-actions-integration)
- [API Reference](#-api-reference)
- [Examples](#-examples)
- [Troubleshooting](#-troubleshooting)
- [License](#-license)
- [Author](#-author)
- [Acknowledgments](#-acknowledgments)

## 🎯 Overview

Unity Builder Action is a modular build system that extends GameCI's Unity Actions with custom build logic. It provides:

- **Platform-specific builders** - Separate builder classes for each platform (WebGL, Android, iOS, etc.)
- **Flexible option parsing** - Automatic parsing of command-line arguments using attributes
- **Dual build support** - Built-in support for multiple builds per platform (e.g., desktop + mobile WebGL)
- **Extensible architecture** - Easy to add new platforms or customize existing ones
- **Detailed logging** - Comprehensive build information and reporting

## ✨ Features

### Core Features

- **Modular architecture** - Clean separation of concerns with platform-specific implementations
- **Automatic argument parsing** - Uses `[Option]` attributes for declarative option definitions
- **Build reporting** - Detailed logging of build configuration and results
- **Error handling** - Proper exit codes and error messages for CI/CD integration
- **Version management** - Automatic version and build number updates

### Current Platform Support

- **WebGL** - Desktop (DXT) and Mobile (ASTC) builds with configurable WASM optimization

### Extensibility

- **Easy platform addition** - Add new platforms by creating a builder class and options
- **Custom build logic** - Override methods to implement platform-specific build steps
- **Flexible reporting** - Customize build information logging per platform

## 📦 Installation

### Manual Installation

1. Copy the entire `Editor` folder into your Unity project's `Assets/Editor` directory (or merge with existing `Assets/Editor/UnityBuilderAction/`)
2. Ensure the assembly definition (`Gamenator.UnityBuilder.Editor.asmdef`) is properly configured
3. The build system is ready to use with GameCI Unity Builder action

**Note:** This is not a Unity package. Files should be placed directly in `Assets/Editor/` or in an Editor Assembly Definition folder.

## 🚀 Quick Start

### GitHub Actions Setup

Create `.github/workflows/build.yml`:

```yaml
name: Build Unity Project

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    name: Build WebGL
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          lfs: true

      - uses: actions/cache@v3
        with:
          path: Library
          key: Library-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-

      - name: Build project
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          projectPath: .
          buildTarget: WebGL
          buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
          customParameters: |
            -codeOptimization RuntimeSpeed
            -isMobile true
            -buildVersion 1.0.0
            -isDevelopmentBuild false
```

The `buildMethod` parameter tells GameCI to use our custom build script instead of the default Unity builder. The `customParameters` are passed as command-line arguments to `BuildScript.BuildMain()`.

## 📖 Usage

### Command-Line Arguments

The build system accepts command-line arguments in the format `-key value` or `-flag` (for booleans). These are passed via the `customParameters` field in GitHub Actions.

#### Common Arguments (All Platforms)

- `-projectPath <path>` (required) - Path to Unity project (usually provided by GameCI)
- `-buildTarget <target>` (required) - Unity build target (WebGL, Android, iOS, etc.) (usually provided by GameCI)
- `-customBuildPath <path>` (required) - Custom output path (usually provided by GameCI)
- `-buildVersion <version>` (optional) - Build version string (e.g., "1.0.0")
- `-androidVersionCode <code>` (optional) - Android version code (integer)
- `-isDevelopmentBuild` (optional) - Enable development build

#### WebGL-Specific Arguments

- `-codeOptimization <level>` (required) - WASM optimization: `RuntimeSpeed` or `Size`
- `-isMobile` (optional) - Build for mobile WebGL (ASTC) in addition to desktop (DXT)

### Example GitHub Actions Configuration

```yaml
# WebGL desktop only
- uses: game-ci/unity-builder@v4
  with:
    buildTarget: WebGL
    buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
    customParameters: |
      -codeOptimization RuntimeSpeed

# WebGL desktop + mobile
- uses: game-ci/unity-builder@v4
  with:
    buildTarget: WebGL
    buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
    customParameters: |
      -codeOptimization RuntimeSpeed
      -isMobile

# With version
- uses: game-ci/unity-builder@v4
  with:
    buildTarget: WebGL
    buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
    customParameters: |
      -codeOptimization RuntimeSpeed
      -buildVersion ${{ github.run_number }}
```

## ➕ Adding New Arguments

This section explains how to add new command-line arguments to the build system. You can add arguments either globally (for all platforms) or platform-specifically.

### Adding Global Arguments (All Platforms)

To add an argument that works for all platforms, add it to `OptionsBase`:

**`Editor/Core/Input/OptionsBase.cs`:**

```csharp
public class OptionsBase
{
    // ... existing options ...

    /// <summary>
    /// Your custom option description.
    /// </summary>
    [Option("myCustomOption", false, 0, "defaultValue")]
    public string MyCustomOption { get; private set; }
}
```

**Supported Types:**
- `string` - For text values
- `int` - For numeric values
- `bool` - For flags (no value needed, just `-flag` in command line)
- `enum` - For enum values (must be convertible)

**OptionAttribute Parameters:**
- `key` (string) - The command-line argument key (e.g., "myCustomOption" for `-myCustomOption value`)
- `isRequired` (bool) - Whether this option is required
- `exitCode` (int) - Exit code to use if required option is missing (only used if `isRequired` is true)
- `defaultValue` (object) - Default value if option is not provided

**Example Usage in GitHub Actions:**
```yaml
customParameters: |
  -myCustomOption myValue
```

### Adding Platform-Specific Arguments

To add an argument for a specific platform, add it to the platform's options class:

**`Editor/WebGL/Input/OptionsWebGL.cs`:**

```csharp
public class OptionsWebGL : OptionsBase
{
    // ... existing WebGL options ...

    /// <summary>
    /// Custom WebGL-specific option.
    /// </summary>
    [Option("webglCustomSetting", false, 0, "default")]
    public string WebglCustomSetting { get; private set; }
}
```

### Using Arguments in Builder Classes

Once you've added an argument, you can access it in your builder class via `ParsedOptions`:

**`Editor/WebGL/BuilderWebGL.cs`:**

```csharp
protected override BuildResult BuildLogic()
{
    // Access the parsed option
    string customValue = ParsedOptions.WebglCustomSetting;
    
    // Use it in your build logic
    if (customValue == "special")
    {
        // Do something special
    }
    
    // ... rest of build logic ...
}
```

### Examples by Type

#### String Argument

```csharp
[Option("outputName", false, 0, "MyGame")]
public string OutputName { get; private set; }
```

Usage: `-outputName MyCustomName`

#### Integer Argument

```csharp
[Option("maxBuildTime", false, 0, 300)]
public int MaxBuildTime { get; private set; }
```

Usage: `-maxBuildTime 600`

#### Boolean Argument (Flag)

```csharp
[Option("enableFeature", false, 0, false)]
public bool EnableFeature { get; private set; }
```

Usage: `-enableFeature` (no value needed, just the flag)

#### Enum Argument

```csharp
public enum QualityLevel { Low, Medium, High }

[Option("qualityLevel", false, 0, QualityLevel.Medium)]
public QualityLevel QualityLevel { get; private set; }
```

Usage: `-qualityLevel High`

#### Required Argument

```csharp
[Option("apiKey", true, 150, null)]
public string ApiKey { get; private set; }
```

If this argument is missing, the build will fail with exit code 150.

### Complete Example: Adding a Custom Build Name Argument

**Step 1: Add to OptionsBase**

```csharp
// In Editor/Core/Input/OptionsBase.cs
[Option("customBuildName", false, 0, "DefaultBuild")]
public string CustomBuildName { get; private set; }
```

**Step 2: Use in Builder**

```csharp
// In Editor/WebGL/BuilderWebGL.cs
protected override BuildResult BuildLogic()
{
    string buildName = ParsedOptions.CustomBuildName;
    string outputPath = $"build/WebGL/{buildName}";
    
    return Build(BuildTarget, BaseBuildOptions, 0, outputPath).result;
}
```

**Step 3: Use in GitHub Actions**

```yaml
customParameters: |
  -customBuildName Release_v1.0.0
```

### Secret Arguments (Hidden in Logs)

Arguments with sensitive data (passwords, API keys) are automatically hidden in logs if their key is in the secrets list. The current secrets list includes:
- `androidKeystorePass`
- `androidKeyaliasName`
- `androidKeyaliasPass`

To add more secrets, edit `Editor/Core/Input/OptionsParser.cs`:

```csharp
private static readonly string[] s_secrets = {
    "androidKeystorePass", 
    "androidKeyaliasName", 
    "androidKeyaliasPass",
    "myApiKey",  // Add your secret here
    "myPassword" // Add more secrets here
};
```

## 🏗️ Architecture

### Project Structure

```
Editor/
├── Core/
│   ├── Builder.cs                    # Abstract base builder class
│   ├── BuildScript.cs                # Main entry point (called by GameCI)
│   ├── Input/
│   │   ├── OptionsBase.cs            # Base options class
│   │   ├── OptionsParser.cs          # Argument parser
│   │   └── ArgumentsParser.cs        # Type-specific parsers
│   ├── Reporting/
│   │   ├── StdInReporter.cs          # Build start reporter
│   │   ├── StdOutReporter.cs          # Build result reporter
│   │   └── BuildOptionsGetter.cs      # Base options formatter
│   ├── Utils/
│   │   ├── BuilderUtils.cs           # Build utilities
│   │   └── BuildOptionsExtensions.cs  # BuildOptions extensions
│   └── Exceptions/
│       └── ArgumentMissingException.cs
├── WebGL/
│   ├── BuilderWebGL.cs               # WebGL builder implementation
│   ├── Input/
│   │   └── OptionsWebGL.cs           # WebGL-specific options
│   └── Reporting/
│       └── BuildOptionsGetterWebGL.cs # WebGL options formatter
├── Common/
│   └── Attributes/
│       └── OptionAttribute.cs         # Option attribute for properties
└── Gamenator.UnityBuilder.Editor.asmdef
```

### Key Components

1. **Builder<T1, T2, T3>** - Abstract base class implementing the template method pattern
2. **OptionsBase** - Base class for all platform option configurations
3. **OptionsParser** - Reflection-based argument parser using `[Option]` attributes
4. **BuildScript** - Main entry point that selects the appropriate builder (called by GameCI via `buildMethod`)

### How It Works with GameCI

According to the [GameCI Builder documentation](https://game.ci/docs/github/builder), the `buildMethod` parameter allows you to specify a custom static method to run your build:

```yaml
buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
```

This method must:
- Be `static`
- Reside in an Editor Assembly (which our code does)
- Accept command-line arguments via `Environment.GetCommandLineArgs()`

Our `BuildScript.BuildMain()` method:
1. Parses command-line arguments using `OptionsParser`
2. Determines the build target
3. Creates the appropriate platform-specific builder
4. Executes the build with platform-specific logic

## 🔧 Extending to New Platforms

This section provides step-by-step instructions for adding support for a new platform (e.g., Android, iOS, Windows).

### Step 1: Create Platform Options Class

Create a new options class inheriting from `OptionsBase`:

**`Editor/Android/Input/OptionsAndroid.cs`:**

```csharp
using Gamenator.Core.UnityBuilder.Core.Input;
using UnityEditor.Android;

namespace Gamenator.Core.UnityBuilder.Android.Input
{
    /// <summary>
    /// Android-specific build options.
    /// </summary>
    public class OptionsAndroid : OptionsBase
    {
        /// <summary>
        /// Android build system (Gradle, Internal).
        /// </summary>
        [Option("androidBuildSystem", false, 0, AndroidBuildSystem.Gradle)]
        public AndroidBuildSystem BuildSystem { get; private set; }

        /// <summary>
        /// Whether to create an Android App Bundle (.aab) instead of APK.
        /// </summary>
        [Option("androidCreateAppBundle", false, 0, false)]
        public bool CreateAppBundle { get; private set; }

        /// <summary>
        /// Keystore password (hidden in logs).
        /// </summary>
        [Option("androidKeystorePass", false, 0, "")]
        public string KeystorePassword { get; private set; }
    }
}
```

### Step 2: Create Platform Builder Class

Create a builder class inheriting from `Builder<T1, T2, T3>`:

**`Editor/Android/BuilderAndroid.cs`:**

```csharp
using Gamenator.Core.UnityBuilder.Core;
using Gamenator.Core.UnityBuilder.Core.Reporting;
using Gamenator.Core.UnityBuilder.Android.Input;
using Gamenator.Core.UnityBuilder.Android.Reporting;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Android;

namespace Gamenator.Core.UnityBuilder.Android
{
    /// <summary>
    /// Android platform-specific builder.
    /// </summary>
    public class BuilderAndroid : Builder<StdInReporter, StdOutReporter, OptionsAndroid>
    {
        protected override BuildTarget BuildTarget => BuildTarget.Android;

        protected override StdInReporter CreateStdInReporter() 
            => new StdInReporter(new BuildOptionsGetterAndroid());

        protected override StdOutReporter CreateStdOutReporter() 
            => new StdOutReporter(new BuildOptionsGetterAndroid());

        protected override OptionsAndroid CreateParsedOptions() 
            => new OptionsAndroid();

        protected override BuildResult BuildLogic()
        {
            // Configure Android-specific settings
            EditorUserBuildSettings.androidBuildSystem = ParsedOptions.BuildSystem;
            EditorUserBuildSettings.buildAppBundle = ParsedOptions.CreateAppBundle;

            // Set keystore password if provided
            if (!string.IsNullOrEmpty(ParsedOptions.KeystorePassword))
            {
                PlayerSettings.Android.keystorePass = ParsedOptions.KeystorePassword;
            }

            // Perform the build
            var outputPath = ParsedOptions.CreateAppBundle 
                ? "build/Android/MyApp.aab" 
                : "build/Android/MyApp.apk";

            var result = Build(
                BuildTarget, 
                BaseBuildOptions, 
                0, // No subtarget for Android
                outputPath
            );

            return result.result;
        }

        protected override void ApplySettings()
        {
            base.ApplySettings();
            // Add Android-specific settings here
        }
    }
}
```

### Step 3: Create Platform Options Getter (Optional)

Create a custom options getter for detailed logging:

**`Editor/Android/Reporting/BuildOptionsGetterAndroid.cs`:**

```csharp
using Gamenator.Core.UnityBuilder.Core.Reporting;
using Gamenator.Core.UnityBuilder.Utils;
using UnityEditor;
using UnityEditor.Android;

namespace Gamenator.Core.UnityBuilder.Android.Reporting
{
    /// <summary>
    /// Android-specific build options getter.
    /// </summary>
    public class BuildOptionsGetterAndroid : BuildOptionsGetter
    {
        public override string GetBuildOptionsString(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            return base.GetBuildOptionsString(buildTarget, buildOptions) +
                $"EditorUserBuildSettings.androidBuildSystem: {EditorUserBuildSettings.androidBuildSystem}{BuilderUtils.EOL}" +
                $"EditorUserBuildSettings.buildAppBundle: {EditorUserBuildSettings.buildAppBundle}{BuilderUtils.EOL}" +
                $"PlayerSettings.Android.targetArchitectures: {PlayerSettings.Android.targetArchitectures}{BuilderUtils.EOL}";
        }
    }
}
```

### Step 4: Register Builder in BuildScript

Update `Core/BuildScript.cs` to include your new platform:

```csharp
var builder = optionsParser.ParseProperty(optionsBase, o => o.BuildTarget) switch
{
    BuildTarget.WebGL => new BuilderWebGL(),
    BuildTarget.Android => new BuilderAndroid(), // Add this line
    // BuildTarget.iOS => new BuilderIOS(),
    // BuildTarget.StandaloneWindows64 => new BuilderWindows(),
    _ => null
};
```

### Step 5: Update GitHub Actions Workflow

Add Android build job to your workflow:

```yaml
jobs:
  build-android:
    name: Build Android
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          lfs: true

      - uses: actions/cache@v3
        with:
          path: Library
          key: Library-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-

      - name: Build Android
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          projectPath: .
          buildTarget: Android
          buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
          customParameters: |
            -androidBuildSystem Gradle
            -androidCreateAppBundle true
            -buildVersion 1.0.0
            -androidVersionCode 1
```

### Complete Example: iOS Builder

Here's a complete example for iOS:

**`Editor/iOS/Input/OptionsIOS.cs`:**

```csharp
using Gamenator.Core.UnityBuilder.Core.Input;

namespace Gamenator.Core.UnityBuilder.iOS.Input
{
    public class OptionsIOS : OptionsBase
    {
        [Option("iosSdkVersion", false, 0, "DeviceSDK")]
        public string SdkVersion { get; private set; }
    }
}
```

**`Editor/iOS/BuilderIOS.cs`:**

```csharp
using Gamenator.Core.UnityBuilder.Core;
using Gamenator.Core.UnityBuilder.Core.Reporting;
using Gamenator.Core.UnityBuilder.iOS.Input;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Gamenator.Core.UnityBuilder.iOS
{
    public class BuilderIOS : Builder<StdInReporter, StdOutReporter, OptionsIOS>
    {
        protected override BuildTarget BuildTarget => BuildTarget.iOS;

        protected override StdInReporter CreateStdInReporter() 
            => new StdInReporter(new BuildOptionsGetter());

        protected override StdOutReporter CreateStdOutReporter() 
            => new StdOutReporter(new BuildOptionsGetter());

        protected override OptionsIOS CreateParsedOptions() 
            => new OptionsIOS();

        protected override BuildResult BuildLogic()
        {
            var result = Build(
                BuildTarget,
                BaseBuildOptions,
                0,
                "build/iOS"
            );
            return result.result;
        }
    }
}
```

## 🔗 GitHub Actions Integration

### Basic Workflow

```yaml
name: Build Unity Project

on: [push, pull_request]

jobs:
  build:
    name: Build ${{ matrix.targetPlatform }}
    runs-on: ubuntu-latest
    strategy:
      matrix:
        targetPlatform:
          - WebGL
          - Android
    steps:
      - uses: actions/checkout@v4
        with:
          lfs: true

      - uses: actions/cache@v3
        with:
          path: Library
          key: Library-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-

      - name: Build project
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          projectPath: .
          buildTarget: ${{ matrix.targetPlatform }}
          buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
          customParameters: |
            -buildVersion ${{ github.run_number }}
            -isDevelopmentBuild false
```

### WebGL with Mobile Build

```yaml
- name: Build WebGL (Desktop + Mobile)
  uses: game-ci/unity-builder@v4
  env:
    UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
    UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
    UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
  with:
    projectPath: .
    buildTarget: WebGL
    buildMethod: Gamenator.Core.UnityBuilder.BuildScript.BuildMain
    customParameters: |
      -codeOptimization RuntimeSpeed
      -isMobile true
      -buildVersion ${{ github.run_number }}
```

### Using GameCI's Built-in Parameters

GameCI's `unity-builder` action automatically provides some parameters via command-line. Our system uses these:

- `-projectPath` - Set by GameCI from the `projectPath` parameter
- `-buildTarget` - Set by GameCI from the `buildTarget` parameter  
- `-customBuildPath` - Set by GameCI based on `buildsPath` and `buildName`

You can override or extend these in `customParameters`, but typically you only need to add platform-specific options.

## 📚 API Reference

### BuildScript

Main entry point for Unity build automation.

#### Methods

- `static void BuildMain()` - Main build entry point called by GameCI Unity Builder action via `buildMethod` parameter.

### Builder<T1, T2, T3>

Abstract base class for platform-specific builders.

#### Abstract Methods

- `BuildTarget BuildTarget { get; }` - The Unity build target
- `T1 CreateStdInReporter()` - Creates input reporter
- `T2 CreateStdOutReporter()` - Creates output reporter
- `T3 CreateParsedOptions()` - Creates options instance
- `BuildResult BuildLogic()` - Implements platform-specific build logic

#### Protected Methods

- `BuildSummary Build(...)` - Performs Unity build with specified parameters
- `void ApplySettings()` - Applies build settings (override for platform-specific settings)
- `BuildOptions GetBuildOptions()` - Gets base build options (override to add platform-specific options)

### OptionsBase

Base class for all platform option configurations.

#### Properties

- `string ProjectPath` (required) - Unity project path
- `BuildTarget BuildTarget` (required) - Unity build target
- `string CustomBuildPath` (required) - Custom output path
- `string BuildVersion` (optional) - Build version string
- `int AndroidVersionCode` (optional) - Android version code
- `bool IsDevelopmentBuild` (optional) - Development build flag

### OptionAttribute

Attribute for marking properties as command-line options.

```csharp
[Option("key", isRequired, exitCode, defaultValue)]
public Type PropertyName { get; private set; }
```

## 💡 Examples

### Dual Build (Desktop + Mobile)

The WebGL builder supports building for both desktop and mobile in a single run:

```yaml
customParameters: |
  -codeOptimization RuntimeSpeed
  -isMobile true
```

This creates:
- `build/WebGL/WebGL/` - Desktop build (DXT compression)
- `build/WebGL/MobileWebGL/` - Mobile build (ASTC compression)

### Custom Build Path

```yaml
customParameters: |
  -customBuildPath build/MyCustomPath
  -buildVersion 2.0.0
```

### Development Build

```yaml
customParameters: |
  -isDevelopmentBuild
  -buildVersion dev-${{ github.sha }}
```

## 🔍 Troubleshooting

### Build Fails with "Unsupported build target"

**Problem:** The build target is not registered in `BuildScript.cs`.

**Solution:** Add your platform builder to the switch statement in `BuildScript.BuildMain()`.

### Missing Required Arguments

**Problem:** Build fails with "Missing argument -key".

**Solution:** Ensure all required arguments (marked with `isRequired: true` in `[Option]`) are provided in `customParameters`.

### Mobile Build Not Working

**Problem:** Mobile WebGL build doesn't run.

**Solution:** Mobile builds require Unity 2021.2 or newer. Check your Unity version.

### Build Options Not Applied

**Problem:** Platform-specific settings aren't being applied.

**Solution:** Override `ApplySettings()` in your builder class to set platform-specific settings.

### BuildMethod Not Found

**Problem:** GameCI can't find the build method.

**Solution:** 
- Ensure the class is in an Editor Assembly Definition
- Verify the namespace and method name are correct: `Gamenator.Core.UnityBuilder.BuildScript.BuildMain`
- Check that the method is `static` and `public`

## 📝 License

MIT License - see LICENSE file for details.

## 👤 Author

**Vitalii Novosad**

- GitHub: [@v17alya](https://github.com/v17alya)

## 🙏 Acknowledgments

- [GameCI](https://game.ci/) for the excellent Unity Actions framework
- Unity Technologies for Unity Editor API
