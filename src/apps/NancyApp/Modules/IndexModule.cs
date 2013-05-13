namespace NancyApp
{
    using Nancy;

    public class IndexModule : NancyModule
    {
        IModuleService _moduleService;

        public IndexModule(IModuleService moduleService)
        {
            _moduleService = moduleService;

            Get["/raw-content"] = inputModel => string.Format("Hi there!<br />IModuleService say: {0}", _moduleService.SuprizeMe());
        }
    }
}
