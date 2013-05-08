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
            new Program(new KatanaHost()).Go(args);
        }

        private EventWaitHandle _abortEvent;
        private BaseAppHost _httpHost;
        private bool _isVerboseExceptions;
        private bool _isSut;

        public Program(BaseAppHost httpHost)
        {
            _httpHost = httpHost;
        }

        internal Program(BaseAppHost httpHost, bool isSystemUnderTest = false)
            : this(httpHost)
        {
            _isSut = isSystemUnderTest;
        }

        public void Go(string[] args)
        {
            _abortEvent = _isSut ? null : new ManualResetEvent(false);

            Parser.Run(args, this);
        }

        partial void Run(int port)
        {
            Console.WriteLine("Running HTTP listener on {0} port...", port);
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
