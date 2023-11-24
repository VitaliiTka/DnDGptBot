using DnDGptBot.DTO;
using DnDGptBot.BL.Services;
using DnDGptBot.BL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

var services = new ServiceCollection();

services.Configure<ChatOptions>(o => configuration.GetSection("OpenAI").Bind(o));
services.AddHttpClient<IOpenAIService, OpenAIService>(client =>
{
    client.BaseAddress = new Uri(configuration["OpenAIBaseUri"]!);
});

services.AddScoped<IDnDService, DnDService>(provider =>
{
    return new DnDService(
        provider.GetRequiredService<IOpenAIService>(),
        configuration["DnDRules"]!);
});

services.AddScoped<ITelegramBotService, TelegramBotService>(provider =>
{
    return new TelegramBotService(
        configuration["TelegramBotToken"]!, 
        provider.GetRequiredService<IDnDService>());
});
var serviceProvider = services.BuildServiceProvider();

/* TelegramBot start */
var telegramBotService = serviceProvider.GetService<ITelegramBotService>();
if (telegramBotService == null)
    return;

try
{
    await telegramBotService.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}