namespace NancyAppHost
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CLAP;

    public partial class Program
    {
        public static void Main(string[] args)
        {
            new Program().Go(args);
        }

        const int HTTP_LISTENER_DEFAULT_PORT = 8888;
        
        private EventWaitHandle _abortEvent;
        private BaseAppHost _httpHost;
        private bool _isVerboseExceptions;
        private bool _isSut;

        internal SupportedHosts NancyHost
        {
            get;
            private set;
        }

        internal Program(BaseAppHost httpHost = null, bool isSystemUnderTest = false)
        {
            _httpHost = httpHost;
            _isSut = isSystemUnderTest;
        }

        public void Go(string[] args)
        {
            _abortEvent = _isSut ? null : new ManualResetEvent(false);

            Parser.Run(args, this);
        }

        partial void Run([DefaultValue(HTTP_LISTENER_DEFAULT_PORT)]int port, [DefaultValue(SupportedHosts.Katana)]SupportedHosts host)
        {
            if (_isSut)
            {
                NancyHost = host;
            }

            _httpHost = _httpHost ?? host.ResolveHostInstance();

            Console.WriteLine("Running {0} on {1} port...", host, port);
            Console.WriteLine("Use Ctrl+C for terminating");

            RunInSeparateThread(delegate
            {
                _httpHost.StartNancyHost(port);
            });

            if (_isSut)
                return;

            _abortEvent.WaitOne();
        }

        private void RunInSeparateThread(Action action)
        {
            if (_isSut)
            {
                action();
                return;
            }

            Task.Factory
                .StartNew(action, TaskCreationOptions.LongRunning)
                .Wait();
        }

        partial void VerboseExceptions()
        {
            _isVerboseExceptions = true;
        }

        partial void HandleError(ExceptionContext context)
        {
            if (_isSut)
            {
                context.ReThrow = _isSut;
                return;
            }

            Console.WriteLine("oops! :-`(");
            Console.WriteLine(_isVerboseExceptions ? context.Exception.ToString() : context.Exception.Message);

            if (context.Exception is AggregateException & !_isVerboseExceptions)
            {
                (context.Exception as AggregateException).Flatten().Handle(exception =>
                {
                    Console.Write(" - ");
                    Console.WriteLine(_isVerboseExceptions ? exception.ToString() : exception.Message);

                    return true;
                });
            }
        }

        partial void Help(string help)
        {
            Console.WriteLine(help);
        }
    }
}
