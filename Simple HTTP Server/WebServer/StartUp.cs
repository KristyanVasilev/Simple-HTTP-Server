using WebServer.Server;
using WebServer.Server.Responcses;

public class StartUp
{
    public static void Main()
    => new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server"))
        .MapGet("/HTML", new HtmlResponse("<h1>Html responce</h1>"))
        .MapGet("/Redirect", new RedirectResponse("https://softuni.org")))
        .Start();
}