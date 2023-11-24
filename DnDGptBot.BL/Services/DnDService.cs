using DnDGptBot.DTO;
using DnDGptBot.BL.Services.Interfaces;

namespace DnDGptBot.BL.Services;

public class DnDService : IDnDService
{
    private readonly IOpenAIService _openAIService;
    public List<Message> _conversationHistory;

    public DnDService(IOpenAIService openAIService, string dnDRules)
    {
        _openAIService = openAIService;
        _conversationHistory = [new Message { role = "system", content = dnDRules }];
    }

    public void ClearStory()
    {
        _conversationHistory = [_conversationHistory[0]];
    }
    public async Task<string> SendMessage(string message)
    {
        AddUserQuestionToConversation(message);
        return await CreateCompletion();
    }

    private void AddUserQuestionToConversation(string message)
        => _conversationHistory.Add(new Message { role = "user", content = message });

    private async Task<string> CreateCompletion()
    {
        var assistantResponse = await _openAIService.CreateChatCompletion(_conversationHistory);
        _conversationHistory.Add(assistantResponse);
        return assistantResponse.content;
    }
}
