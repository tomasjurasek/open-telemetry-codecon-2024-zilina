using OpenTelemetry.Demo.ServiceDefaults.Models;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace OpenTelemetry.Demo.API.Database
{
    public class RequestStorage : IRequestStorage
    {
        private ConcurrentDictionary<Guid, RequestDTO> _storage { get; set; } = new();

        public async Task StoreAsync(RequestDTO request)
        {
            using var activity = ActivitySourceProvider.ActivitySource.StartActivity(nameof(StoreAsync));
            activity?.AddTag("requst.id", request.Id);

            _storage.AddOrUpdate(request.Id, (_) =>
            {
                activity?.AddEvent(new ActivityEvent("RequestAdded"));
                return request;
            }
            , (_, v) =>
            {
                activity?.AddEvent(new ActivityEvent("RequestModified"));
                v = request;
                return v;

            });
        }

        public async Task<RequestDTO> GetAsync(Guid id)
        {
            using var activity = ActivitySourceProvider.ActivitySource.StartActivity(nameof(GetAsync));
            activity?.SetTag("request.id", id);
            return _storage[id];
        }

        public async Task<IReadOnlyCollection<RequestDTO>> GetAsync()
        {
            using var activity = ActivitySourceProvider.ActivitySource.StartActivity(nameof(GetAsync));
            return _storage.Values.ToList().AsReadOnly();
        }
    }

    public interface IRequestStorage
    {
        Task<RequestDTO> GetAsync(Guid id);
        Task StoreAsync(RequestDTO request);
        Task<IReadOnlyCollection<RequestDTO>> GetAsync();

    }
}
