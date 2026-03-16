namespace BusinessLogic.Service
{
    public interface IOpenAIService
    {
        Task<string> SendPromptAsync(
            string userMessage,
            IEnumerable<(string role, string content)>? history = null);
    }
}