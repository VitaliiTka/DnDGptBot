namespace DnDGptBot.BL.Services.Interfaces;

public interface IDnDService
{
    void ClearStory();
    Task<string> SendMessage(string message);
}
