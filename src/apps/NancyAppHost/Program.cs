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
            try
            {
                ResetEvent = EventWaitHandle.OpenExisting(WAIT_HANDLE_NAME);
                
                IsResetEventAlreadyExists = true;
            }
            catch
            {
                IsResetEventAlreadyExists = false;
            }

            new Program(new MockHost()).Go(args);
        }

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

        partial void Run(int port)
        {
            Console.WriteLine("Running HTTP listener on {0} port...", port);
            Console.WriteLine("Use \"abort\" command line switch for terminating (i.e. NancyAppHost.exe abort)");

            if (IsRunning)
                throw new InvalidOperationException("HTTP listener already runned");

            StrartHttpListenerInSeparateThread(port);

            if (!_isSut)
            {
                ResetEvent.WaitOne();
                ResetEvent.Dispose();
            }
            Console.WriteLine("HTTP listener is aborted.");
        }

        private void StrartHttpListenerInSeparateThread(int port)
        {
            Task.Factory
                .StartNew(delegate
                {
                    HttpHost.StartHttpListener(port);
                }
                , TaskCreationOptions.LongRunning)
                .Wait();
        }

        partial void Abort()
        {
            if (!IsRunning)
                throw new InvalidOperationException("HTTP listener is not runned");

            Console.WriteLine("Trying abort HTTP listener...");
            
            HttpHost.TerminateHttpListener();

            if (!_isSut)
            {
                ResetEvent.Set();
            }
        }

        partial void VerboseExceptions()
        {
            _isVerboseExceptions = true;
        }

        partial void HandleError(ExceptionContext context)
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

        partial void Help(string help)
        {
            Console.WriteLine(help);
        }
    }
}
