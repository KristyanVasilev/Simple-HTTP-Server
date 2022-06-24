using WebServer.Server.HTTP;

namespace WebServer.Server.Responcses
{
    public class NotFoundResponse : Response
    {
        public NotFoundResponse()
            : base(StatusCode.NotFound)
        {
        }
    }
}
