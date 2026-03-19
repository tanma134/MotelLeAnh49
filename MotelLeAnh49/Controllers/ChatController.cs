// MotelLeAnh49/Controllers/ChatController.cs
using BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;
using MotelLeAnh49.Helpers;
namespace MotelLeAnh49.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // GET /Chat
        public IActionResult Index()
        {
            return View();
        }

        // POST /Chat/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
                return BadRequest(new { error = "Message cannot be empty." });

            // 🔥 Lấy CustomerId từ Session
            int? customerId = HttpContext.Session.GetInt32("CustomerId");

            // convert sang string (vì ChatService đang dùng string)
            string? userId = customerId?.ToString();

            var aiResponse = await _chatService.ProcessUserMessageAsync(request.Message, customerId);

            // 🔥 Nếu là guest → lưu session
            if (customerId == null)
            {
                var history = HttpContext.Session.GetObject<List<string>>("ChatHistory") ?? new();

                history.Add("User: " + request.Message);
                history.Add("AI: " + aiResponse);

                HttpContext.Session.SetObject("ChatHistory", history);
            }

            return Ok(new { response = aiResponse });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}