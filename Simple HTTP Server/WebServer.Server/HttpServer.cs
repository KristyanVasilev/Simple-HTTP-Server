using System.Net;
using System.Net.Sockets;
using System.Text;
using WebServer.Server.HTTP;
using WebServer.Server.Routing;

namespace WebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;
        private readonly RoutingTable routingTable;


        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            this.serverListener = new TcpListener(this.ipAddress, port);

            routingTableConfiguration(this.routingTable = new RoutingTable());
        }

        public HttpServer(int port, Action<IRoutingTable> routingTable)
            :this("127.0.0.1", port, routingTable)
        {
        }

        public HttpServer(Action<IRoutingTable> routingTable)
            : this("127.0.0.1", 8080, routingTable)
        {
        }


        public void Start()
        {
            this.serverListener.Start();

            Console.WriteLine($"Server started on port:{port}.");
            Console.WriteLine("Listening for request...");

            while (true)
            {
                var connection = serverListener.AcceptTcpClient();

                var networkStream = connection.GetStream();

                var requestTxt = this.ReadRequest(networkStream);

                Console.WriteLine(requestTxt);

                var request = Request.Parse(requestTxt);

                var response = this.routingTable.MathcRequest(request);
                WriteResponce(networkStream, response);

                connection.Close();
            }
        }
        //public async Task Start()
        //{
        //    this.serverListener.Start();

        //    Console.WriteLine($"Server started on port {port}.");
        //    Console.WriteLine("Listening for requests...");

        //    while (true)
        //    {
        //        var connection = serverListener.AcceptTcpClientAsync();

        //        _ = Task.Run(() =>
        //        {
        //            var networkStream = connection.GetStream();

        //            var requestText = this.ReadRequest(networkStream);

        //            Console.WriteLine(requestText);

        //            var request = Request.Parse(requestText, ServiceCollection);

        //            var response = this.routingTable.MatchRequest(request);

        //            AddSession(request, response);

        //            WriteResponse(networkStream, response);

        //            connection.Close();
        //        });
        //    }
        //}

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

        private static void WriteResponce(NetworkStream networkStream, Response response)
        {          

            byte[] responceByte = Encoding.UTF8.GetBytes(response.ToString());
            networkStream.Write(responceByte);
        }
    }
}
