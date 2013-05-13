namespace NancyApp
{
    using System.IO;
    using System.Reflection;
    using Nancy;
    using Nancy.Bootstrappers.StructureMap;
    using StructureMap;

    public sealed class NancyAppBootstrapper : StructureMapNancyBootstrapper
    {
        protected override void ConfigureRequestContainer(IContainer container, NancyContext context)
        {
            container.Configure(_ =>
            {
                _.Scan(c =>
                {
                    c.AssembliesFromApplicationBaseDirectory();
                    c.WithDefaultConventions();
                    c.LookForRegistries();
                });
            });
        }

        protected override IRootPathProvider RootPathProvider
        {
            get
            {
                return new NancyAppRootPathProvider();
            }
        }

        private sealed class NancyAppRootPathProvider : IRootPathProvider
        {
            public string GetRootPath()
            {
                var assembly = Assembly.GetEntryAssembly();

                return assembly != null ?
                    Path.GetDirectoryName(assembly.Location) :
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }
    }
}
