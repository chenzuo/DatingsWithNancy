namespace NancyAppHost.Tests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ProgramTests
    {
        [Fact]
        void Http_Listener_Exceptions_Not_Handled_While_System_Under_Test()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var programProd = Substitute.For<Program>(httpHost);      // public Program(BaseAppHost)
            var programSut = Substitute.For<Program>(httpHost, true); // internal Program(BaseAppHost, isSystemUnderTest = false)
            
            httpHost
                .WhenForAnyArgs(x => x.StartHttpListener(000))
                .Do(x => { throw new ApplicationException("Something goes wrong"); });

            Action actProd = () => programProd.Go("run".Split());
            Action actSut = () => programSut.Go("run".Split());

            actProd.ShouldNotThrow();
            actSut
                .ShouldThrow<AggregateException>()
                .And
                .InnerExceptions.Should().Contain(x => x.Message.Equals("Something goes wrong"));
        }

        [Fact]
        void Http_Listener_Default_Port_Is_8888()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var program = Substitute.For<Program>(httpHost, true);

            program.Go("run".Split());

            httpHost
                .Received()
                .StartHttpListener(Arg.Is(8888));
        }

        [Fact]
        void Only_One_Program_Instance_At_A_Time_Can_Be_Runned()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var program1 = Substitute.For<Program>(httpHost, true);
            var program2 = Substitute.For<Program>(httpHost, true);
            var program3 = Substitute.For<Program>(httpHost, true);

            program1.IsRunning.Returns(true);
            program2.IsRunning.Returns(false);
            program3.IsRunning.Returns(true);

            Action act1 = () => program1.Go("run".Split());
            Action act2 = () => program2.Go("run -port=1234".Split());
            Action act3 = () => program3.Go("run".Split());

            act1.ShouldThrow<InvalidOperationException>().WithMessage("HTTP listener already runned");
            act2.ShouldNotThrow();
            act3.ShouldThrow<InvalidOperationException>().WithMessage("HTTP listener already runned");

            httpHost
                .Received(1)
                .StartHttpListener(Arg.Is(1234));
        }

        [Fact]
        void Program_Can_Only_Be_Aborted_While_It_Is_Running()
        {
            var httpHost = Substitute.For<BaseAppHost>();
            var program = Substitute.For<Program>(httpHost, true);
            
            program.IsRunning.Returns(false, true, false);
            Action act1 = () => program.Go("run".Split());
            Action act2 = () => program.Go("abort".Split());
            Action act3 = () => program.Go("abort".Split());

            act1.ShouldNotThrow();
            act2.ShouldNotThrow();
            act3.ShouldThrow<InvalidOperationException>().WithMessage("HTTP listener is not runned");

            httpHost
                .ReceivedWithAnyArgs(1)
                .StartHttpListener(000);

            httpHost
                .ReceivedWithAnyArgs(1)
                .TerminateHttpListener();
        }
    }
}
