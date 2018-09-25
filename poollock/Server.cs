using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;

namespace poollock
{
    class Server
    {
        public Server()
        {
        }

        public async void Run(IPEndPoint endPoint)
        {
            TcpListener listener = new TcpListener(endPoint);
            listener.Start(1000);

            while (true)
            {
                var conn = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Connection accepted");

                Task _ = Task.Run(() => HandleConnection(conn));
            }
        }

        static readonly ReadOnlyMemory<byte> s_requestEnd = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes("\r\n\r\n"));
        static readonly ReadOnlyMemory<byte> s_response = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: 11\r\n\r\nHello World"));

        public async void HandleConnection(TcpClient conn)
        {
            try
            {
                using (var stream = conn.GetStream())
                {
                    Memory<byte> buffer = new Memory<byte>(new byte[4096]);
                    while (true)
                    {
                        int offset = 0;
                        // Read request
                        while (true)
                        {
                            int bytesRead = await stream.ReadAsync(buffer.Slice(offset));
                            if (bytesRead == 0)
                            {
                                Console.WriteLine($"Connection closed");
                                return;
                            }

//                            Console.WriteLine($"Read {bytesRead} bytes");

                            offset += bytesRead;
                            if (offset >= s_requestEnd.Length && 
                                buffer.Slice(offset - s_requestEnd.Length, s_requestEnd.Length).Span.SequenceEqual(s_requestEnd.Span))
                            {
                                break;
                            }

                            if (offset == buffer.Length)
                            {
                                throw new Exception("request too long");
                            }
                        }

//                        Console.WriteLine($"Sending response");

                        // Send response
                        await stream.WriteAsync(s_response);
                    }
                }
            }
            catch (IOException)
            {
                // eat it
            }

        }
    }
}
