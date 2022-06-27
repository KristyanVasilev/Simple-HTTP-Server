using System.Web;

namespace WebServer.Server.HTTP
{
    public class Request
    {
        public Method Method { get; set; }
        public string Url { get; set; }

        public HeaderCollection Headers { get; set; }
        public string Body { get; set; }

        public IReadOnlyDictionary<string, string> Form { get; private set; }


        public static Request Parse(string request)
        {
            var lines = request.Split("\r\n");

            var startLine = lines.First().Split(" ");

            var method = ParseMethod(startLine[0]);
            var url = startLine[1];

            var headers = ParseHeaders(lines.Skip(1));

            var bodyLines = lines.Skip(headers.Count + 2).ToArray();

            var body = string.Join("\r\n", bodyLines);

            var form = ParseForm(headers, body);

            return new Request
            {
                Method = method,
                Url = url,
                Headers = headers,
                Body = body,
                Form = form
            };
        }

        private static Dictionary<string, string> ParseForm(HeaderCollection headers, string body)
        {
            var formCollection = new Dictionary<string, string>();

            if (headers.Contains(Header.ContentType)
                && headers[Header.ContentType] == ContentType.FormUrlEncoded)
            {
                var parsedResult = ParseFormData(body);

                foreach (var (name, value) in parsedResult)
                {
                    formCollection.Add(name, value);
                }
            }

            return formCollection;
        }


        private static Dictionary<string, string> ParseFormData(string bodyLines)
          => HttpUtility.UrlDecode(bodyLines)
              .Split('&')
              .Select(part => part.Split('='))
              .Where(part => part.Length == 2)
              .ToDictionary(
                  part => part[0],
                  part => part[1],
                  StringComparer.InvariantCultureIgnoreCase);

        private static HeaderCollection ParseHeaders(IEnumerable<string> headersLines)
        {
            var headerCollection = new HeaderCollection();

            foreach (var line in headersLines)
            {
                if (line == string.Empty)
                {
                    break;
                }

                var headersParts = line.Split(':', 2);

                if (headersParts.Length != 2)
                {
                    throw new InvalidOperationException("Request is not valid");
                }

                var headerName = headersParts[0];
                var headerValue = headersParts[1].Trim();

                headerCollection.Add(headerName, headerValue);
            }

            return headerCollection;
        }

        private static Method ParseMethod(string method)
        {
            try
            {
                return (Method)Enum.Parse(typeof(Method), method, true);
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Method '{method} is not supported'");
            }
        }
    }
}
