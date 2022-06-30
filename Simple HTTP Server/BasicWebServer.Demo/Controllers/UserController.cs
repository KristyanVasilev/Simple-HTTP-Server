using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System;

namespace BasicWebServer.Demo.Controllers
{
    public class UserController : Controller
    {

        private const string Username = "user";

        private const string Password = "user123";

        public UserController(Request request)
            : base(request)
        {
        }

        public Response LoginUser()
        {
            Request.Session.Clear();

            var bodyText = String.Empty;

            var usernameMatches = Request.Form["Username"] == Username;
            var passwordMatches = Request.Form["Password"] == Password;

            if (usernameMatches && passwordMatches)
            {
                Request.Session[Session.SessionUserKey] = "MyUserId";
                var cookies = new CookieCollection();
                cookies.Add(Session.SessionCookieName,
                    Request.Session.Id);

                bodyText = "<h3>Logged successfully!</h3>";

                return Html(bodyText, cookies);
            }

            return Redirect("/Login");
        }

        public Response GetUserData()
        {
            if (Request.Session.ContainsKey(Session.SessionUserKey))
            {
                return Html($"<h3>Currently logged-in user is with username '{Username}'</h3>");
            }

            return Redirect("/Login");
        }

        public Response LogoutUser()
        {
            Request.Session.Clear();

            return Html("<h3>Logged out successfully!</h3>");
        }

        public Response Login() => View();
    }
}
