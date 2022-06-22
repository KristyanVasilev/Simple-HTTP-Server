using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicWebServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var port = 8080;

            var serverListener = new TcpListener(ipAddress, port);

            serverListener.Start();

            Console.WriteLine($"Server started on port:{port}.");
            Console.WriteLine("Listening for request...");

            while (true)
            {
                var connection = serverListener.AcceptTcpClient();

                var networkStream = connection.GetStream();
                var content = "Hello from the server!";
                var contentLenght = Encoding.UTF8.GetByteCount(content);

                var response = $"HTTP/1.1 200 OK Server: StivanServer 2020 " +
                    $"Content-Type: text/plain; charset=utf-8 Content-Length: {contentLenght}";

                byte[] responceByte = Encoding.UTF8.GetBytes(response);

                networkStream.Write(responceByte);

                connection.Close();
            }
        }
    }
}
