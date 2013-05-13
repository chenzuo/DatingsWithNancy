namespace NancyAppHost
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Nancy.Hosting.Wcf;

    public sealed class WcfHost : BaseAppHost
    {
        public override void StartNancyHost(int port)
        {
            var uri = new Uri(string.Format("http://localhost:{0}/", port));
            var host = new WebServiceHost(new NancyWcfGenericService(Bootstrapper), uri);

            host.AddServiceEndpoint(typeof(NancyWcfGenericService), new WebHttpBinding(), string.Empty);
            host.Open();
        }
    }
}
