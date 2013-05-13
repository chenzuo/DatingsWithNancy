namespace NancyAppHost.Tests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ProgramTests
    {
        [Fact]
        void Program_Exceptions_Is_Not_Handling_By_Clap_While_System_Is_Under_Test()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var program = Substitute.For<Program>(httpHost, true);
            
            httpHost
                .WhenForAnyArgs(x => x.StartNancyHost(000))
                .Do(x => { throw new ApplicationException("Something goes wrong"); });

            Action act = () => program.Go("run".Split());

            act.ShouldThrow<ApplicationException>().WithMessage("Something goes wrong");
        }

        [Fact]
        void Program_Default_Listening_Port_Is_8888()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var program = Substitute.For<Program>(httpHost, true);

            program.Go("run".Split());

            httpHost
                .Received()
                .StartNancyHost(Arg.Is(8888));
        }

        [Fact]
        void Program_Should_Run_Nancy_Host_At_Port_Specified_In_Command_Line_Arguments()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var program = Substitute.For<Program>(httpHost, true);

            program.Go("run -port=1234".Split());

            httpHost
                .Received(1)
                .StartNancyHost(Arg.Is(1234));
        }

        [Fact]
        void Program_Default_Http_Host_Is_Katana()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var program = Substitute.For<Program>(httpHost, true);

            program.Go("run".Split());

            program.NancyHost.Should().Be(SupportedHosts.Katana);
        }

        [Fact]
        void Program_Should_Throw_Exception_When_Used_Unsupported_Nancy_Host()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var program = Substitute.For<Program>(httpHost, true);

            Action act = () => program.Go("run -host=Drabadan".Split());

            act.ShouldThrow<Exception>()
                .Where(ex => ex.Message.Contains("'Drabadan' cannot be converted to NancyAppHost.SupportedHosts"));
        }
    }
}
