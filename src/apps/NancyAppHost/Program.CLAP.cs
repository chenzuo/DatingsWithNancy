namespace NancyAppHost
{
    using CLAP;

    public partial class Program
    {
        [Verb(Description = "Run HTTP listener on specified port number"), ]
        partial void Run(int port, SupportedHosts host);

        [Global(Description = "Print full exception's callstack on any fault", Aliases = "verbose")]
        partial void VerboseExceptions();

        [Error]
        partial void HandleError(ExceptionContext context);

        [Empty, Help]
        partial void Help(string help);
    }
}
