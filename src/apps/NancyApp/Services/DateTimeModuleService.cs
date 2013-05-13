namespace NancyApp
{
    using System;

    public class DateTimeModuleService : IModuleService
    {
        public string SuprizeMe()
        {
            return DateTime.Now.ToString();
        }
    }
}
