namespace BusinessLogic.Config
{
    public class OpenAIConfig
    {
        public string ApiKey { get; set; } = "";
        public string Model { get; set; } = "";
        public int MaxTokens { get; set; }
        public string SystemPrompt { get; set; } = "";
    }
}
