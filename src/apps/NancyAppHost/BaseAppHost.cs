namespace NancyAppHost
{
    public abstract class BaseAppHost
    {
        public abstract void StartNancyHost(int port);
        
        private string ResolveNancyBootstrapperAssembly()
        {
            return string.Empty;
        }
    }
}
