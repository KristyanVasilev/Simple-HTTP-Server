using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;


        public HttpServer(string ipAddress, int port)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            this.serverListener = new TcpListener(this.ipAddress, port);
        }

        public void Start()
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

                var requestTxt = this.ReadRequest(networkStream);
                Console.WriteLine(requestTxt);

                WriteResponce(networkStream, "Hello from the server!");

                connection.Close();
            }
        }

        private string ReadRequest(NetworkStream networkStream)
        {
            var bufferLenght = 1024;
            var buffer = new byte[bufferLenght];
            var totalBytes = 0;

            var requestBuilder = new StringBuilder();

            do
            {
                var bytesRead = networkStream.Read(buffer, 0, bufferLenght);

                totalBytes += bytesRead;

                if (totalBytes > 10 * 1024)
                {
                    throw new InvalidOperationException("Request is too large!");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                //May not work correctly over the interner
            } while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }

        private static void WriteResponce(NetworkStream networkStream, string message)
        {
            var contentLenght = Encoding.UTF8.GetByteCount(message);

            var response = $"HTTP/1.1 200 OK Server: StivanServer 2020 " +
                $"Content-Type: text/plain; charset=utf-8 Content-Length: {contentLenght} {message}";

            byte[] responceByte = Encoding.UTF8.GetBytes(response);
            networkStream.Write(responceByte);
        }
    }
}
