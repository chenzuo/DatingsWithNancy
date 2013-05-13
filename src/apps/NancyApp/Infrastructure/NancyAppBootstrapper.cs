namespace NancyApp
{
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
    }
}
