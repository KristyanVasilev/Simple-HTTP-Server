using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System;
using System.Linq;
using System.Text;
using System.Web;

namespace BasicWebServer.Demo.Controllers
{
    public class HomeController : Controller
    {

        private const string DownloadForm = @"<form action='/Content' method='POST'>
                <input type='submit' value ='Download Sites Content' /> 
            </form>";

        private const string FileName = "content.txt";

        private const string HtmlForm = @"<form action='/HTML' method='POST'>
            Name: <input type='text' name='Name'/>
            Age: <input type='number' name ='Age'/>
            <input type='submit' value ='Save' />
        </form>";

        public HomeController(Request request)
            :base(request)
        {
        }

        public Response Index() => Text("Hello from the server!");
        public Response Redirect() => Redirect("https://softuni.org");
        public Response Html() => Html(HtmlForm);
        public Response HtmlFormPost()
        {
            var sb = new StringBuilder();

            foreach (var (key, value) in Request.Form)
            {
                sb.AppendLine($"{key} - {value}");
            }

            return Text(sb.ToString());
        }

        public Response DownloadContent() => File(FileName);

        public Response Content() => Html(DownloadForm);

        public Response Cookies()
        {
            var requestHasCookies = Request.Cookies.Any(c => c.Name != Session.SessionCookieName);
            var bodyText = String.Empty;

            if (requestHasCookies)
            {
                var cookieText = new StringBuilder();
                cookieText.AppendLine("<h1>Cookies</h1>");

                cookieText
                    .Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (var cookie in Request.Cookies)
                {
                    cookieText.Append("<tr>");
                    cookieText
                        .Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieText
                        .Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieText.Append("</tr>");
                }
                cookieText.Append("</table>");

                bodyText = cookieText.ToString();
                return Html(bodyText);
            }
            else
            {
                bodyText = "<h1>Cookies set!</h1>";
            }

            var cookies = new CookieCollection();

            if (!requestHasCookies)
            {
                cookies.Add("My-Cookie", "My-Value");
                cookies.Add("My-Second-Cookie", "My-Second-Value");
            }

            return Html(bodyText, cookies);
        }
    }
}
