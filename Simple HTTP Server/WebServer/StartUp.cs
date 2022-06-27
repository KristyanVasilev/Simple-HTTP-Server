using System.Text;
using System.Web;
using WebServer.Server;
using WebServer.Server.HTTP;
using WebServer.Server.Responcses;

public class StartUp
{
    private const string HtmlForm = @"<form action='/HTML' method='POST'>
           Name: <input type='text' name='Name'/>
           Age: <input type='number' name ='Age'/>
           <input type='submit' value ='Save' />
        </form>";

    private const string DownloadForm = @"<form action='/Content' method='POST'> <input type='submit' value ='Download Sites Content' /> </form>";

    private const string FileName = "content.txt";

    private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";

    private const string username = "user";
    private const string password = "user123";


    public static async Task Main()
    {
        await DownloadSitesAsTextFile(StartUp.FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

        var server = new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server"))
        .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
        .MapGet("/HTML", new HtmlResponse(StartUp.HtmlForm))
        .MapPost("/HTML", new TextResponse("", StartUp.AddFromDataAction))
        .MapGet("/Content", new HtmlResponse(StartUp.DownloadForm))
        .MapPost("/Content", new FileResponse(StartUp.FileName))
        .MapGet("/Cookies", new HtmlResponse("", StartUp.AddCookiesAction))
        .MapGet("/Session", new TextResponse("", StartUp.DisplaySessionInfo))
        .MapGet("/Login", new HtmlResponse(StartUp.HtmlForm))
        .MapGet("/Login", new HtmlResponse("", StartUp.LoginAction))
        .MapGet("/Logout", new HtmlResponse("", StartUp.LogoutAction))
        .MapGet("/UserProfile", new HtmlResponse("", StartUp.GetUserDataAction)));

       await server.Start();

    }

    private static void GetUserDataAction(Request request, Response response)
    {
        if (request.Session.ContainsKey(Session.SessionUserKey))
        {
            response.Body = "";
            response.Body += $"<h1>Currently logged-in user with username {username}</h1>";
        }
        else
        {
            response.Body = "";
            response.Body += $"<h1>You should first logged-in!</h1>";
        }
    }

    private static void LogoutAction(Request request, Response response)
    {
        request.Session.Clear();

        response.Body = "";
        response.Body += "<h1>Logget out successfully!</h1>";
    }

    private static void LoginAction(Request request, Response response)
    {
        request.Session.Clear();
        var body = "";

        var usernameMatches = request.Form["Username"] == StartUp.username;
        var passwordMatches = request.Form["Password"] == StartUp.password;

        if (usernameMatches && passwordMatches)
        {
            request.Session[Session.SessionUserKey] = "MyUserId";
            response.Cookies.Add(Session.SessionCookieName, request.Session.Id);

            body = "<h1>Logged successfully!</h1>";
        }
        else
        {
            body = StartUp.LoginForm;
        }

        response.Body = "";
        response.Body += body;
    }

    private static void DisplaySessionInfo(Request request, Response response)
    {
        var sessionExist = request.Session.ContainsKey(Session.SessionCurrentDateKey);
        var body = "";

        if (sessionExist)
        {
            var currDate = request.Session[Session.SessionCurrentDateKey];
            body = $"Stored date: {currDate}";
        }
        else
        {
            body = "Current date stored";
        }

        response.Body = "";
        response.Body += body;
    }

    private static void AddCookiesAction(Request request, Response response)
    {
        bool requstHasCookies = request.Cookies
            .Any(c => c.Name != Session.SessionCookieName);
        string body = "";

        if (requstHasCookies)
        {
            var cookiesTxt = new StringBuilder();
            cookiesTxt.AppendLine("<h1>Cookies</h1>");

            cookiesTxt.Append("<table border='1'><tr><th>Name</th><th>Vale</th></tr>");

            foreach (var cookie in request.Cookies)
            {
                cookiesTxt.Append("<tr>");
                cookiesTxt.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                cookiesTxt.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                cookiesTxt.Append("</tr>");
            }
            cookiesTxt.AppendLine("</table>");

            body = cookiesTxt.ToString();
        }
        else
        {
            body = "<h1>Cookies set!</h1>";

            response.Cookies.Add("My-Cookie", "SecretValue");
            response.Cookies.Add("My-Cookie2", "Value");
        }

        response.Body = body;
    }

    private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
    {
       var downloads = new List<Task<string>>();

        foreach (var url in urls)
        {
            downloads.Add(DownloadWebSiteContent(url));
        }

        var response = await Task.WhenAll(downloads);
        var responseString = string.Join(Environment.NewLine + new String('-', 100), response);

        await File.WriteAllTextAsync(fileName, responseString);
    }

    private static async Task<string> DownloadWebSiteContent(string url)
    {
        var httpClient = new HttpClient();
        using(httpClient)
        {
            var response = await httpClient.GetAsync(url);

            var html = await response.Content.ReadAsStringAsync();

            return html.Substring(0, 2000);
        }
    }

    private static void AddFromDataAction(Request request, Response response)
    {
        response.Body = "";

        foreach (var (key, value) in request.Form)
        {
            response.Body += $"{key} - {value}";
            response.Body += Environment.NewLine;
        }
    }
}