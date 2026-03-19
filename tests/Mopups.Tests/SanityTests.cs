using Xunit;

namespace Mopups.Tests;

public class SanityTests
{
    [Fact]
    public void Sanity_Check_Test_Runner_Works()
    {
        // This is a basic sanity check test
        Assert.True(true);
    }

    [Fact]
    public void Basic_Xunit_Functionality_Works()
    {
        // Verify xUnit is working correctly
        var result = 1 + 1;
        Assert.Equal(2, result);
    }

    [Fact]
    public void Test_Project_Builds_Successfully()
    {
        // Verify the test project itself compiles
        var message = "Test infrastructure is set up";
        Assert.NotNull(message);
        Assert.Contains("Test", message);
    }
}
