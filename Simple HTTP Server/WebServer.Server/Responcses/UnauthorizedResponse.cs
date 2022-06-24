using WebServer.Server.HTTP;

namespace WebServer.Server.Responcses
{
    public class UnauthorizedResponse : Response
    {
        public UnauthorizedResponse() : 
            base(StatusCode.Unauthorized)
        {
        }
    }
}
