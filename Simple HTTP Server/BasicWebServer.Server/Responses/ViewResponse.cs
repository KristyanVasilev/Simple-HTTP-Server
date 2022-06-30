using BasicWebServer.Server.HTTP;
using System;
using System.IO;

namespace BasicWebServer.Server.Responses
{
    public class ViewResponse : ContentResponse
    {
        private const char PathSeparathor = '/';
        public ViewResponse(string viewName, string controllerName)
            : base(String.Empty, ContentType.Html)
        {
            if (!viewName.Contains(PathSeparathor))
            {
                viewName = controllerName + PathSeparathor + viewName;
            }

            var viewPath = Path.GetFullPath($"./Views/{viewName.TrimStart(PathSeparathor)}.cshtml");
            var viewContent = File.ReadAllText(viewPath);

            this.Body = viewContent;
        }
    }
}
