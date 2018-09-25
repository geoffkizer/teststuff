using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading;

namespace poollock
{
    class Program
    {
        const int s_port = 5000;
        const int s_clientCount = 1000;

        static readonly HttpClient s_httpClient = new HttpClient();

        static async void RunClient()
        {
            while (true)
            {
                try
                {
                    var response = await s_httpClient.GetAsync("http://localhost:5000/");
                    response.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Client exception: {e}");
                }
            }
        }

        static void Main(string[] args)
        {
            var s = new Server();
            s.Run(new IPEndPoint(IPAddress.Loopback, s_port));

            for (int i = 0; i < s_clientCount; i++)
            {
                Thread.Sleep(10);
                Task.Run(() => RunClient());
            }

            Console.WriteLine("All clients running");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
