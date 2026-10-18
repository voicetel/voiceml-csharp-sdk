using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace VoiceML.Tests;

public sealed class ReadmeExamplesTests
{
    [Fact]
    public void EveryCSharpReadmeBlockMatchesTheCompiledExample()
    {
        var readme = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Readme.md"));
        var example = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Quickstart.txt"));
        var blocks = Regex.Matches(readme, "```csharp\\r?\\n(.*?)```", RegexOptions.Singleline);
        Assert.Single(blocks);
        Assert.Equal(Regex.Replace(example, @"\s+", "").Trim(), Regex.Replace(blocks[0].Groups[1].Value, @"\s+", "").Trim());
    }
}
