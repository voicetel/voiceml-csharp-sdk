# Installing VoiceML

## From nuget.org

```sh
dotnet add package VoiceML
```

[`VoiceML`](https://www.nuget.org/packages/VoiceML) is published by the `voicetel` organization on nuget.org. Each release is packed and pushed by this repository's publish workflow using trusted publishing, and the package's `.nuspec` records the exact source commit (`<repository … commit="…" />`).

## From source

Use a source build to try changes that are not yet released. The .NET 10 SDK builds both `net8.0` and `net10.0` assets.

### Local NuGet feed

Run from a directory where you want the checkout and feed:

```sh
git clone https://github.com/voicetel/voiceml-csharp-sdk.git
dotnet pack voiceml-csharp-sdk/src/VoiceML/VoiceML.csproj -c Release -o "$PWD/voiceml-local-feed"
dotnet new console -n MyVoiceApp -f net10.0
version=$(dotnet msbuild voiceml-csharp-sdk/src/VoiceML/VoiceML.csproj -getProperty:Version)
dotnet add MyVoiceApp/MyVoiceApp.csproj package VoiceML --version "$version" --source "$PWD/voiceml-local-feed"
dotnet build MyVoiceApp/MyVoiceApp.csproj -c Release
```

The explicit `--source` selects your local feed rather than nuget.org. A .NET 8 application can use `-f net8.0` instead; use .NET 10 for new applications because .NET 8 support ends on 2026-11-10.

### ProjectReference

After cloning the repository, create a console project and reference the library:

```sh
dotnet new console -n MyVoiceApp -f net10.0
dotnet add MyVoiceApp/MyVoiceApp.csproj reference voiceml-csharp-sdk/src/VoiceML/VoiceML.csproj
dotnet build MyVoiceApp/MyVoiceApp.csproj -c Release
```

See the [README](../README.md) for a complete compiling example. Source and package links are maintained at [the SDK repository](https://github.com/voicetel/voiceml-csharp-sdk); API reference documentation is at <https://voicetel.com/docs/api/v0.9/voiceml/>.
