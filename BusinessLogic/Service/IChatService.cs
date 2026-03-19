// BusinessLogic/Service/IChatService.cs
namespace BusinessLogic.Service
{
    public interface IChatService
    {
        Task<string> ProcessUserMessageAsync(string userMessage, int? customerId);
    }
}