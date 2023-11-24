using DnDGptBot.DTO;

namespace DnDGptBot.BL.Services.Interfaces;

public interface IOpenAIService
{
    Task<Message> CreateChatCompletion(List<Message> messages);
}
