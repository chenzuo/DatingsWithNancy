namespace NancyApp
{
    using StructureMap.Configuration.DSL;

    public sealed class NancyAppRegistry : Registry
    {
        public NancyAppRegistry()
        {
            For<IModuleService>().Use<DateTimeModuleService>();
        }
    }
}
