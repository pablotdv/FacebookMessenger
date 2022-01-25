using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FacebookMessenger.Middlewares
{
    public class VerificarAssinaturaMiddleware
    {
        private const string appSecret = "ac755e6cd18fb6bcb0bfda2963ac834e";

        private readonly RequestDelegate _next;

        public VerificarAssinaturaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "POST" && context.Request.Path.StartsWithSegments("/api/Webhook"))
            {
                var req = context.Request;
                var assinatura = req.Headers["x-hub-signature"].ToString();

                req.EnableBuffering();
                using StreamReader reader = new(req.Body, leaveOpen: true);
                var bodyStr = await reader.ReadToEndAsync();
                req.Body.Position = 0;

                byte[] keyBytes = new UTF8Encoding().GetBytes(appSecret);
                using HMACSHA1 sha1 = new HMACSHA1(keyBytes);
                var hash = sha1.ComputeHash(new UTF8Encoding().GetBytes(bodyStr));
                var gerado = Convert.ToHexString(hash);

                if (!assinatura.EndsWith(gerado, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }            

            await _next(context);
        }
    }
}
