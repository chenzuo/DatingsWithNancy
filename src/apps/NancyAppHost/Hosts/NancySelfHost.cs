namespace NancyAppHost
{
    using System;
    using Nancy.Hosting.Self;

    public sealed class NancySelfHost : BaseAppHost
    {
        public override void StartNancyHost(int port)
        {
            var uri = new Uri(string.Format("http://localhost:{0}", port));
            
            new NancyHost(Bootstrapper, uri).Start();
        }
    }
}
