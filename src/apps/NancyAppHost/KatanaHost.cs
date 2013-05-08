﻿namespace NancyAppHost
{
    using Microsoft.Owin.Hosting;
    using Owin;

    public class KatanaHost : BaseAppHost
    {
        public override void StartNancyHost(int port)
        {
            WebApplication.Start(port, app => app.UseNancy(Bootstrapper));
        }
    }
}
