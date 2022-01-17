using System.Threading.Tasks;

namespace IoT.Simulator.Services
{
    public interface ITelemetryMessageService : IMessageService
    {
        Task<string> GetRandomizedMessageAsync(string deviceId, string moduleId, Direction direction);
    }
}
