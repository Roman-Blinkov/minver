using System.Reflection;
using System.Threading.Tasks;
using MinVerTests.Infra;
using Xunit;

namespace MinVerTests.Packages;

public static class Skip
{
    [Fact]
    public static async Task HasDefaultSdkVersion()
    {
        // arrange
        var path = MethodBase.GetCurrentMethod().GetTestDirectory();
        await Sdk.CreateProject(path);
        var envVars = ("MinVerSkip".ToAltCase(), "true");
        var expected = Package.WithVersion(1, 0, 0);

        // act
        var (actual, _, _) = await Sdk.BuildProject(path, envVars);

        // assert
        Assert.Equal(expected, actual);
    }
}
