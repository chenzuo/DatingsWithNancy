namespace NancyAppHost
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CLAP;

    public class Program
    {
        public static void Main(string[] args)
        {
            IsResetEventAlreadyExists = EventWaitHandle.TryOpenExisting(WAIT_HANDLE_NAME, out ResetEvent);

            new Program(new MockHost()).Go(args);
        }

        private const int HTTP_LISTENER_DEFAULT_PORT = 8888;
        private const string WAIT_HANDLE_NAME = "Global\\NancyAppHost.8888";

        private static EventWaitHandle ResetEvent;
        private static BaseAppHost HttpHost;
        private static bool? IsResetEventAlreadyExists;

        private bool _isVerboseExceptions = false;
        private bool _isSut = false;

        internal virtual bool IsRunning
        {
            get
            {
                return IsResetEventAlreadyExists.Value;
            }
        }
        
        public Program(BaseAppHost httpHost)
        {
            HttpHost = httpHost;
        }

        internal Program(BaseAppHost httpHost, bool isSystemUnderTest = false)
            : this(httpHost)
        {
            _isSut = isSystemUnderTest;
            
            // while SUT mock IsRunnig property which defines desired behavior
            IsResetEventAlreadyExists = false;
        }

        public void Go(string[] args)
        {
            ResetEvent = _isSut ? null : new EventWaitHandle(false, EventResetMode.AutoReset, WAIT_HANDLE_NAME);

            Parser.Run(args, this);
        }

        #region CLAP Verbs

        [Verb(Description = "Run HTTP listener on specified port number")]
        private void Run([DefaultValue(HTTP_LISTENER_DEFAULT_PORT)]int port)
        {
            Console.WriteLine("Running HTTP listener on {0} port...", port);

            if (IsRunning) throw new InvalidOperationException("HTTP listener already runned");

            Task.Factory
                .StartNew(delegate
                {
                    HttpHost.StartHttpListener(port);
                }
                , TaskCreationOptions.LongRunning)
                .Wait();
            
            Console.WriteLine("Use \"abort\" command line switch for terminating (i.e. NancyAppHost.exe abort)");

            if (!_isSut)
            {
                ResetEvent.WaitOne();
                ResetEvent.Dispose();
            }
            Console.WriteLine("HTTP listener is aborted.");
        }

        [Verb(Description = "Terminate HTTP listener")]
        private void Abort()
        {
            if (!IsRunning) throw new InvalidOperationException("HTTP listener is not runned");

            Console.WriteLine("Trying abort HTTP listener...");
            HttpHost.TerminateHttpListener();

            if (!_isSut)
            {
                ResetEvent.Set();
            }
        }

        [Global(Description = "Print full exception's callstack on any fault", Aliases = "verbose")]
        private void VerboseExceptions()
        {
            _isVerboseExceptions = true;
        }

        [Error]
        private void HandleError(ExceptionContext context)
        {
            context.ReThrow = _isSut;

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

        [Empty, Help]
        private void Help(string help)
        {
            Console.WriteLine(help);
        }

        #endregion
    }
}
