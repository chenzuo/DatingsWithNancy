namespace NancyAppHost
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Extensions;
    using Nancy.IO;
    using NHttp;

    public sealed class NHttpHost : BaseAppHost
    {
        public override void StartNancyHost(int port)
        {
            var requestHandler = new NancyRequestHandler(Bootstrapper);

            var host = new HttpServer()
            {
                EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port)
            };

            host.RequestReceived += (o, e) =>
            {
                requestHandler.ProcessRequest(e.Context);
            };

            host.Start();
        }

        private sealed class NancyRequestHandler
        {
            readonly INancyEngine _nancyEngine;
            
            public NancyRequestHandler(INancyBootstrapper bootstrapper)
            {
                bootstrapper.Initialise();

                _nancyEngine = bootstrapper.GetEngine();
            }

            public void ProcessRequest(HttpContext context)
            {
                var request = CreateNancyRequest(context);

                using (var nancyContext = _nancyEngine.HandleRequest(request))
                {
                    SetNancyResponseToHttpResponse(context, nancyContext.Response);
                }
            }

            private static Request CreateNancyRequest(HttpContext context)
            {
                var incomingHeaders = context.Request.Headers.ToDictionary();

                var expectedRequestLength = GetExpectedRequestLength(incomingHeaders);

                var basePath = string.Empty;

                var path = context.Request.Url.AbsolutePath.Substring(basePath.Length);
                path = string.IsNullOrWhiteSpace(path) ? "/" : path;

                var nancyUrl = new Url
                {
                    Scheme = context.Request.Url.Scheme,
                    HostName = context.Request.Url.Host,
                    Port = context.Request.Url.Port,
                    BasePath = basePath,
                    Path = path,
                    Query = context.Request.Url.Query,
                    Fragment = context.Request.Url.Fragment,
                };

                return new Request(
                    context.Request.HttpMethod.ToUpperInvariant(),
                    nancyUrl,
                    RequestStream.FromStream(context.Request.InputStream, expectedRequestLength, true),
                    incomingHeaders,
                    context.Request.UserHostAddress);
            }

            private static long GetExpectedRequestLength(IDictionary<string, IEnumerable<string>> incomingHeaders)
            {
                if (incomingHeaders == null)
                {
                    return 0;
                }

                if (!incomingHeaders.ContainsKey("Content-Length"))
                {
                    return 0;
                }

                var headerValue = incomingHeaders["Content-Length"].SingleOrDefault();

                if (headerValue == null)
                {
                    return 0;
                }

                long contentLength;
                if (!long.TryParse(headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength))
                {
                    return 0;
                }

                return contentLength;
            }

            private static void SetNancyResponseToHttpResponse(HttpContext context, Response response)
            {
                SetHttpResponseHeaders(context, response);

                if (response.ContentType != null)
                {
                    context.Response.ContentType = response.ContentType;
                }
                context.Response.StatusCode = (int)response.StatusCode;
                response.Contents.Invoke(context.Response.OutputStream);
            }

            private static void SetHttpResponseHeaders(HttpContext context, Response response)
            {
                foreach (var header in response.Headers.ToDictionary(x => x.Key, x => x.Value))
                {
                    context.Response.Headers.Add(header.Key, header.Value);
                }

                foreach (var cookie in response.Cookies.ToArray())
                {
                    context.Response.Headers.Add("Set-Cookie", cookie.ToString());
                }
            }
        }
    }
}
