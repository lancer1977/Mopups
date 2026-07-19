using System.Xml.Linq;
using Xunit;

namespace Mopups.Tests;

public class MopupsProjectConfigurationTests
{
    [Theory]
    [InlineData("-android", "Platforms/Android/**/*.cs")]
    [InlineData("-ios", "Platforms/iOS/**/*.cs")]
    [InlineData("-maccatalyst", "Platforms/MacCatalyst/**/*.cs")]
    [InlineData("-windows", "Platforms/Windows/**/*.cs")]
    public void PlatformSpecificSources_AreExcludedWhenTargetFrameworkDoesNotMatchPlatform(
        string platformSuffix,
        string platformGlob)
    {
        var itemGroups = LoadProject()
            .Root!
            .Elements("ItemGroup")
            .Where(group => ((string?)group.Attribute("Condition"))?.Contains($"$(TargetFramework.Contains('{platformSuffix}')) != true") == true);

        Assert.Contains(
            itemGroups,
            group => group.Elements("Compile").Any(element => (string?)element.Attribute("Remove") == platformGlob));
    }

    [Fact]
    public void PlatformExcludes_UseDirectoryGlobsThatMatchRepositoryLayout()
    {
        var compileRemoves = LoadProject()
            .Descendants("Compile")
            .Select(element => (string?)element.Attribute("Remove"))
            .Where(value => value is not null)
            .ToArray();

        Assert.DoesNotContain("**\\**\\*.Android.cs", compileRemoves);
        Assert.DoesNotContain("**\\**\\*.iOS.cs", compileRemoves);
        Assert.DoesNotContain("**\\**\\*.MacCatalyst.cs", compileRemoves);
        Assert.DoesNotContain("**\\*.Windows.cs", compileRemoves);
    }

    [Fact]
    public void PlatformSources_DoNotContainCommentOnlyPlaceholders()
    {
        var platformRoot = Path.GetFullPath(
            Path.Combine(
                RepositoryRoot,
                "Mopups",
                "Mopups.Maui",
                "Platforms"));

        var sourceFiles = Directory.GetFiles(platformRoot, "*.cs", SearchOption.AllDirectories);

        Assert.All(sourceFiles, path =>
        {
            var hasCode = File.ReadLines(path)
                .Select(line => line.Trim().TrimStart('\uFEFF'))
                .Any(line => line.Length > 0 && !line.StartsWith("//", StringComparison.Ordinal));

            Assert.True(hasCode, $"{Path.GetRelativePath(RepositoryRoot, path)} should contain real source or be removed.");
        });
    }

    private static XDocument LoadProject()
    {
        var projectPath = Path.GetFullPath(
            Path.Combine(
                RepositoryRoot,
                "Mopups",
                "Mopups.Maui",
                "Mopups.csproj"));

        return XDocument.Load(projectPath);
    }

    private static string RepositoryRoot => Path.GetFullPath(
        Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            ".."));
}
