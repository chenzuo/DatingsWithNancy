namespace NancyAppHost
{
    using CLAP;

    public partial class Program
    {
        const int HTTP_LISTENER_DEFAULT_PORT = 8888;
        
        [Verb(Description = "Run HTTP listener on specified port number"), ]
        partial void Run(
            
            [DefaultValue(HTTP_LISTENER_DEFAULT_PORT)]
            int port,

            [DefaultValue(SupportedHosts.Katana)]
            SupportedHosts host
        );

        [Global(Description = "Print full exception's callstack on any fault", Aliases = "verbose")]
        partial void VerboseExceptions();

        [Error]
        partial void HandleError(ExceptionContext context);

        [Empty, Help]
        partial void Help(string help);
    }
}
