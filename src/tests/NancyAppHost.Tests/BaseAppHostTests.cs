namespace NancyAppHost.Tests
{
    using System;
    using FluentAssertions;
    using Nancy;
    using Nancy.Bootstrapper;
    using NSubstitute;
    using Xunit;
    
    public class BaseAppHostTests
    {
        [Fact]
        void BaseAppHost_Bootstrapper_Property_Should_Return_DefaultNancyBootstrapper_When_Type_Is_Undefined_Or_Not_Found()
        {
            var httpHost = Substitute.For<BaseAppHost>();

            httpHost.Bootstrapper.Should()
                .NotBeNull()
                .And
                .BeOfType<DefaultNancyBootstrapper>();
        }

        [Fact]
        void BaseAppHost_Bootstrapper_Property_Should_Return_Correct_Resolved_Type()
        {
            var httpHost = Substitute.For<BaseAppHost>();

            httpHost
                .GetFirstTypeNameFromConfiguration()
                .Returns(typeof(NancyBootstrapperDouble).AssemblyQualifiedName);

            httpHost.Bootstrapper.Should()
                .NotBeNull()
                .And
                .BeAssignableTo<INancyBootstrapper>();
        }

        #region Test Doubles

        private class NancyBootstrapperDouble : INancyBootstrapper
        {
            public Nancy.INancyEngine GetEngine()
            {
                throw new NotImplementedException();
            }

            public void Initialise()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
