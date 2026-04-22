using DenonRemote.Models;

namespace DenonRemote.Services.Interfaces;

public interface IReceiverStateService
{
    ReceiverState CurrentState { get; }
    event Action<ReceiverState>? StateChanged;
}
