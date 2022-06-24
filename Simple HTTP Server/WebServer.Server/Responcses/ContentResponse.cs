﻿using System.Text;
using WebServer.Server.Common;
using WebServer.Server.HTTP;

namespace WebServer.Server.Responcses
{
    public class ContentResponse : Response
    {

        public ContentResponse(string content, string contentType)
            : base(StatusCode.OK)
        {
            Guard.AgainstNull(content);
            Guard.AgainstNull(contentType);

            this.Headers.Add(Header.ContentType, contentType);

            this.Body = content;
        }

        public override string ToString()
        {
            if (this.Body != null)
            {
                var contentLenght = Encoding.UTF8.GetByteCount(this.Body).ToString();
                this.Headers.Add(Header.ContentLenght, contentLenght);
            }

            return base.ToString();
        }
    }
}
