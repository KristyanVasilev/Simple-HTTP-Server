using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;

namespace BasicWebServer.Server.Controllers
{
    public class Controller
    {

        public Controller(Request request)
        {
            this.Request = request;
        }

        protected Request Request { get; private init; }

        protected Response Text(string text) => new TextResponse(text);
        protected Response Html(string text) => new HtmlResponse(text);
        protected Response BadRequest() => new BadRequestResponse();
        protected Response Unauthorize() => new UnauthorizedResponse();
        protected Response NotFound() => new NotFoundResponse();
        protected Response Redirect(string location) => new RedirectResponse(location);
        protected Response File(string fileName) => new FileResponse(fileName);
    }
}
