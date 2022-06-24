using WebServer.Server.HTTP;

namespace WebServer.Server.Responcses
{
    public class BadRequestResponce : Response
    {
        public BadRequestResponce()
            : base(StatusCode.BadRequest)
        {
        }
    }
}
