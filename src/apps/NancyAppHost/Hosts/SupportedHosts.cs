namespace NancyAppHost
{
    using System;

    public enum SupportedHosts
    {
        Katana = 0,
        Firefly = 1,
        Self = 2,
        Wcf = 3,
        NHttp = 4
    }

    public static class SupportedHostsExtensions
    {
        public static BaseAppHost ResolveHostInstance(this SupportedHosts host)
        {
            Type hostType;

            switch (host)
            {
                case SupportedHosts.Katana:
                    hostType = typeof(KatanaHost);
                    break;
                case SupportedHosts.Firefly:
                    hostType = typeof(FireflyHost);
                    break;
                case SupportedHosts.Self:
                    hostType = typeof(NancySelfHost);
                    break;
                case SupportedHosts.Wcf:
                    hostType = typeof(WcfHost);
                    break;
                case SupportedHosts.NHttp:
                    hostType = typeof(NHttpHost);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Can't resolve host with type {0}", host));
            }

            return Activator.CreateInstance(hostType) as BaseAppHost;
        }
    }
}
