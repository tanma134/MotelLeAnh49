// MotelLeAnh49/Controllers/ChatController.cs
using BusinessLogic.Service;
using Microsoft.AspNetCore.Mvc;

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

            var aiResponse = await _chatService.ProcessUserMessageAsync(request.Message);

            return Ok(new { response = aiResponse });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}