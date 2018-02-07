using System;
using System.Net;
using System.Threading;
using System.Linq;

namespace dnstest
{
    class Program
    {
        static void DumpIPHostEntry(IPHostEntry entry)
        {
            Console.WriteLine($"IPHostEntry for {entry.HostName}");
            
            foreach (var a in entry.AddressList)
            {
                Console.WriteLine($"Address: {a}");
            }

            foreach (var a in entry.Aliases)
            {
                Console.WriteLine($"Alias: {a}");
            }
        }

        static void Main(string[] args)
        {
            var last = Dns.GetHostEntry("microsoft.com");
            DumpIPHostEntry(last);

            while (true)
            {
                Thread.Sleep(500);

                var current = Dns.GetHostEntry("microsoft.com");

                if (!Enumerable.SequenceEqual(current.AddressList, last.AddressList))
                {
                    Console.WriteLine($"{DateTime.UtcNow}: IPHostEntry.AddressList changed");
                    DumpIPHostEntry(current);

                    last = current;
                }
            }
        }
    }
}
