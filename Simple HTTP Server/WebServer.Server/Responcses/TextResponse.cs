using WebServer.Server.HTTP;

namespace WebServer.Server.Responcses
{
    public class TextResponse : ContentResponse
    {
        public TextResponse(string text, Action<Request, Response> preRenderAction = null)
            : base(text, ContentType.PlainText)
        {
        }
    }
}
