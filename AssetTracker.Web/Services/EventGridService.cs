using Azure;
using Azure.Identity;
using Azure.Messaging.EventGrid;
using System.Text.Json;

namespace AssetTracker.Web.Services
{
    public class EventGridService
    {
        private readonly string _endpoint;
        private readonly string _key;

        public EventGridService(IConfiguration config)
        {
            // Values will come from App Service Configuration (or appsettings for local)
            _endpoint = config["EVENTGRID_TOPIC_ENDPOINT"] ?? "";
            _key = config["EVENTGRID_TOPIC_KEY"] ?? "";
        }

        public async Task PublishEventAsync(string eventType, object data)
        {
            if (string.IsNullOrEmpty(_endpoint) || string.IsNullOrEmpty(_key))
                return; // fail-safe: don't crash if not configured

            var client = new EventGridPublisherClient(
            new Uri(_endpoint),
            new AzureKeyCredential(_key)
             ); 

            var evt = new EventGridEvent(
                subject: "AssetTracker/Notification",
                eventType: eventType,          // e.g. "Asset.RepairStarted"
                data: JsonSerializer.Serialize(data),
                dataVersion: "1.0"
            );

            await client.SendEventAsync(evt);
        }
    }
}
