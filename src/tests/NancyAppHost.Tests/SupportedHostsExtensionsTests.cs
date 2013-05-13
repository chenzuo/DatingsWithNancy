namespace NancyAppHost.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class SupportedHostsExtensionsTests
    {
        [Fact]
        void ResolveHostInstance_Should_Throw_InvalidOperationException_When_Used_Undefined_Host_Type()
        {
            var undefinedHostType = (SupportedHosts)int.MaxValue;

            Action act = () => undefinedHostType.ResolveHostInstance();

            act.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        void ResolveHostInstance_For_All_Supported_Hosts_Should_Return_BaseAppHost_Instance()
        {
            Array.ForEach(typeof(SupportedHosts).GetEnumNames(), hostType =>
            {
                SupportedHosts host;
                Enum.TryParse<SupportedHosts>(hostType, out host);

                host.ResolveHostInstance().Should()
                    .NotBeNull()
                    .And
                    .BeAssignableTo<BaseAppHost>();
            });
        }
    }
}
