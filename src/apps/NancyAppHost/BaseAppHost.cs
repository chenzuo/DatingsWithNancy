namespace NancyAppHost
{
    public abstract class BaseAppHost
    {
        public abstract void StartHttpListener(int port);
        
        public abstract void TerminateHttpListener();

        private string ResolveNancyBootstrapperAssembly()
        {
            return string.Empty;
        }
    }
}
