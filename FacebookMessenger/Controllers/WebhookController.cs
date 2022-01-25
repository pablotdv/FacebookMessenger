using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FacebookMessenger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private const string verifyToken = "123123";
        private const string appSecret = "ac755e6cd18fb6bcb0bfda2963ac834e";

        public WebhookController(ILogger<WebhookController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.verify_token")] string token,
            [FromQuery(Name = "hub.challenge")] string challenge)
        {
            if (mode == "subscribe" && token == verifyToken)
            {
                return Ok(challenge);
            }

            return new StatusCodeResult(403);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WebhookRequest request)
        {
            _logger.LogInformation("{Body}", request);
            return Ok("EVENT_RECEIVED");
        }
    }

    public class WebhookRequest
    {
        [JsonPropertyName("object")]
        public string? _object { get; set; }
        public Entry[]? entry { get; set; }
    }

    public class Entry
    {
        public string? id { get; set; }
        public long time { get; set; }
        public Messaging[]? messaging { get; set; }
    }

    public class Messaging
    {
        public Sender? sender { get; set; }
        public Recipient? recipient { get; set; }
        public long timestamp { get; set; }
        public Message? message { get; set; }
    }

    public class Sender
    {
        public string? id { get; set; }
    }

    public class Recipient
    {
        public string? id { get; set; }
    }

    public class Message
    {
        public string? mid { get; set; }
        public string? text { get; set; }
    }

}
