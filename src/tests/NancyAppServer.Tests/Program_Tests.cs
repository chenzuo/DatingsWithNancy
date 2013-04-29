namespace NancyAppServer.Tests
{
    using Xunit;
    using FluentAssertions;

    public class Program_Tests
    {
        [Fact]
        public void When_Call_Start_It_Returns_True()
        {
            var done = new Program().Start();

            done.Should().BeTrue();
        }
    }
}
