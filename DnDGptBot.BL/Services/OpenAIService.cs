using DnDGptBot.DTO;
using DnDGptBot.BL.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace DnDGptBot.BL.Services;

public class OpenAIService(HttpClient httpClient, IOptions<ChatOptions> options) : IOpenAIService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IOptions<ChatOptions> _options = options;

    public async Task<Message> CreateChatCompletion(List<Message> messages)
    {
        var request = new { model = _options.Value.GptModel, messages = messages.ToArray() };

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.Value.ApiKey);
        //return new Message() { content = "asd", role = "bot" };
        var response = await _httpClient.PostAsJsonAsync(_options.Value.ApiUrl, request);
        //response.EnsureSuccessStatusCode();

        var chatCompletionResponse = await response.Content.ReadFromJsonAsync<ChatbotResponse>();
        return chatCompletionResponse?.choices.First().message!;
    }
}
