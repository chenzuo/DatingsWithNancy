namespace NancyAppHost
{
    using System;
    using System.Configuration;
    using System.Linq;
    using Nancy;
    using Nancy.Bootstrapper;

    public abstract class BaseAppHost
    {
        protected internal INancyBootstrapper Bootstrapper
        {
            get
            {
                try
                {
                    var typeName = GetFirstTypeNameFromConfiguration();
                    var type = Type.GetType(typeName, false);

                    return Activator.CreateInstance(type) as INancyBootstrapper;
                }
                catch
                {
                    return new DefaultNancyBootstrapper();
                }
            }
        }

        public abstract void StartNancyHost(int port);

        internal virtual string GetFirstTypeNameFromConfiguration()
        {
            try
            {
                return ConfigurationManager.AppSettings.GetValues("NancyBootstrapper").FirstOrDefault();
            }
            catch
            {
                return default(string);
            }
        }
    }
}
