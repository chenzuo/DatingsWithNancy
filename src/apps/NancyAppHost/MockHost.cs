namespace NancyAppHost
{
    using System;
    using System.Threading.Tasks;

    public class MockHost : BaseAppHost
    {
        public override async void StartNancyHost(int port)
        {
            Console.WriteLine("Using MockHost");

            if (string.Empty == await DoWork())
            {
                throw new ApplicationException("Nooooooooooooooo...!");
            }
        }

        async Task<string> DoWork()
        {
            await Task.Delay(10000);

            return string.Empty;
        }
    }
}
