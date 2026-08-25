using Xunit;
using FinanceTracker.Domain;

namespace ExampleTest;
public class Trivial_AssertTrue
{
    [Fact]
    public void Trivial_TrueIsTrue()
    {
        Assert.True(true);
    }
}