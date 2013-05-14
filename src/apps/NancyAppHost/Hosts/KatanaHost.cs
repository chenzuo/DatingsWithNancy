namespace NancyAppHost
{
    using Microsoft.Owin.Hosting;
    using Owin;

    public sealed class KatanaHost : BaseAppHost
    {
        public override void StartNancyHost(int port)
        {
            var url = string.Format("http://127.0.0.1:{0}/", port);

            WebApplication.Start(url, app => app.UseNancy(Bootstrapper));
        }
    }
}
