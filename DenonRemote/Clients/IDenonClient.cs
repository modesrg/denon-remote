namespace DenonRemote.Clients;

public interface IDenonClient
{
    Task SendCommandAsync(string command);
    Task<string> GetStateXmlAsync();
}
