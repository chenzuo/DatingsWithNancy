namespace NancyAppHost
{
    using Firefly.Http;
    using Nancy.Owin;

    public sealed class FireflyHost : BaseAppHost
    {
        public override void StartNancyHost(int port)
        {
            var owinApp = new NancyOwinHost(null, Bootstrapper);
            
            new ServerFactory().Create(owinApp.Invoke, port, "localhost");
        }
    }
}
