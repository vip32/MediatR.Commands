namespace MediatR.Commands
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public static class HttpResponseExtensions
    {
        public static Task Location(this HttpResponse source, string location = null)
        {
            if (source == null || location.IsNullOrEmpty())
            {
                return Task.CompletedTask;
            }

            if (source.Headers.ContainsKey("Location"))
            {
                source.Headers.Remove("Location");
            }

            source.Headers.Add("Location", location);

            return Task.CompletedTask;
        }

        public static Task Header(this HttpResponse source, string key, string value, bool allowEmptyValue = false)
        {
            if (source == null || key.IsNullOrEmpty() || (value.IsNullOrEmpty() && !allowEmptyValue))
            {
                return Task.CompletedTask;
            }

            if (source.Headers.ContainsKey(key))
            {
                source.Headers.Remove(key);
            }

            source.Headers.Add(key, value);

            return Task.CompletedTask;
        }

        public static Task Ok(this HttpResponse source, object content = null)
        {
            source.StatusCode = (int)HttpStatusCode.OK;
            //if (content != null)
            //{
            //    source.WriteJson(content);
            //}

            return Task.CompletedTask;
        }

        public static Task Created(this HttpResponse source, string location = null)
        {
            source.StatusCode = (int)HttpStatusCode.Created;

            return source.Location(location);
        }

        public static Task Accepted(this HttpResponse source)
        {
            source.StatusCode = (int)HttpStatusCode.Accepted;
            return Task.CompletedTask;
        }

        public static Task NoContent(this HttpResponse source)
        {
            source.StatusCode = (int)HttpStatusCode.NoContent;
            return Task.CompletedTask;
        }

        public static Task BadRequest(this HttpResponse source, string warning = null)
        {
            source.StatusCode = (int)HttpStatusCode.BadRequest;

            if (!source.Headers.ContainsKey("Warning") && !warning.IsNullOrEmpty())
            {
                source.Headers.Add("Warning", $"199 naos \"{warning}\""); // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Warning
            }

            return Task.CompletedTask;
        }

        public static Task NotFound(this HttpResponse source)
        {
            source.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }

        public static Task InternalServerError(this HttpResponse source)
        {
            source.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Task.CompletedTask;
        }
    }
}
