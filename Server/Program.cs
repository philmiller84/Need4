using Need4;
using System.Threading;

namespace Need4Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (ServiceHandler server = new ServiceHandler())
            {
                server.Startup();
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
