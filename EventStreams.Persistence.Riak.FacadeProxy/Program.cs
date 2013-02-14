using System;
using System.Net;
using System.Net.Sockets;
using CommandLine;

namespace EventStreams.Persistence.Riak.FacadeProxy {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("hello");

            Console.WriteLine(typeof(Console).Assembly.CodeBase);

            var udp = new UdpClient(new IPEndPoint(IPAddress.Any, 2222));
            udp.BeginSend(new byte[20], 20, new IPEndPoint(IPAddress.Parse("192.168.1.254"), 2224), OnEndSend, udp);

            //CommandLineParser.Default.ParseArguments(args, null);
            Console.ReadKey();
        }

        static void OnEndSend(IAsyncResult ar) {
            var udp = (UdpClient)ar.AsyncState;
            var num = udp.EndSend(ar);
        }
    }
}
