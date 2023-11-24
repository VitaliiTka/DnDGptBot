using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using DnDGptBot.BL.Services.Interfaces;

namespace DnDGptBot.BL.Services;

public class TelegramBotService(string token, IDnDService dnDService) : ITelegramBotService
{
    private readonly string _token = token;
    private readonly IDnDService _dnDService = dnDService;

    public async Task Start()
    {
        var botClient = new TelegramBotClient(_token);

        using var cts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();
        Console.ReadLine();
        cts.Cancel();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        try
        {
            if (messageText == "/restart")
            {
                _dnDService.ClearStory();
                return;
            }
            var result = await _dnDService.SendMessage(messageText);
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: result,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "some error massage",
                cancellationToken: cancellationToken);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
