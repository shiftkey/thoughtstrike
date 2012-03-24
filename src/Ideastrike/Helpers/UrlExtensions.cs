using System;
using System.Web;
using System.Web.Mvc;

namespace Ideastrike.Helpers
{
    public static class UrlExtensions
    {
        public static string ToAbsoluteUrl(this UrlHelper url, string path)
        {
            return url.ToAbsoluteUrl(HttpContext.Current, path);
        }

        public static string ToAbsoluteUrl(this UrlHelper url, HttpContext httpContext, string path)
        {
            var uriBuilder = new UriBuilder
            {
                Host = httpContext.Request.Url.Host,
                Path = "/",
                Port = 80,
                Scheme = "http",
            };

            if (httpContext.Request.IsLocal)
            {
                uriBuilder.Port = httpContext.Request.Url.Port;
            }

            var relativeUri = new Uri(path, UriKind.Relative);

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }
    }
}