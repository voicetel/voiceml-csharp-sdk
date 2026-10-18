# Installing VoiceML from source

The intended NuGet ID is `VoiceML`, version `0.9.2`, but it is **not yet published on nuget.org**. Use an SDK package built from this repository or reference the source project directly.

## Local NuGet feed

The .NET 10 SDK builds both `net8.0` and `net10.0` assets. Run from a directory where you want the checkout and feed:

```sh
git clone https://github.com/voicetel/voiceml-csharp-sdk.git
dotnet pack voiceml-csharp-sdk/src/VoiceML/VoiceML.csproj -c Release -o "$PWD/voiceml-local-feed"
dotnet new console -n MyVoiceApp -f net10.0
dotnet add MyVoiceApp/MyVoiceApp.csproj package VoiceML --version 0.9.2 --source "$PWD/voiceml-local-feed"
dotnet build MyVoiceApp/MyVoiceApp.csproj -c Release
```

The explicit `--source` selects your local feed. A .NET 8 application can use `-f net8.0` instead; use .NET 10 for new applications because .NET 8 support ends on 2026-11-10.

## ProjectReference

After cloning the repository, create a console project and reference the library:

```sh
dotnet new console -n MyVoiceApp -f net10.0
dotnet add MyVoiceApp/MyVoiceApp.csproj reference voiceml-csharp-sdk/src/VoiceML/VoiceML.csproj
dotnet build MyVoiceApp/MyVoiceApp.csproj -c Release
```

See the [README](../README.md) for a complete compiling example. Source and package links are maintained at [the SDK repository](https://github.com/voicetel/voiceml-csharp-sdk); API reference documentation is at <https://voicetel.com/docs/api/v0.9/voiceml/>.
