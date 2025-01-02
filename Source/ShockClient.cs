using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Celeste.Mod.Shockeline;

public class ShockClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new CustomJsonStringEnumConverter() }
    };

    private readonly HttpClient _httpClient;

    public ShockClient(ApiOptions options)
    {
        // TODO: Make a preflight request to check if we have any currently active shockers, if not, no-op all functionality
        _httpClient = new HttpClient
        {
            BaseAddress = options.Server,
            DefaultRequestHeaders =
            {
                { "OpenShockToken", options.Token }
            }
        };
    }

    public async Task<bool> ControlShocker(Guid guid)
    {
        // TODO: Figure out API v2
        using var controlResponse = await _httpClient.PostAsJsonAsync("1/shockers/control",
            new[]
            {
                new
                {
                    id = guid,
                    type = ShockelineModule.Settings.Type + 1, // enum starts at 0
                    intensity = ShockelineModule.Settings.Intensity,
                    duration = ShockelineModule.Settings.Duration
                }
            });

        var stream = controlResponse.Content.ReadAsStream();
        var res = new StreamReader(stream).ReadToEnd();
        var data = controlResponse.RequestMessage.Content.ReadAsStream();
        var sent = new StreamReader(data).ReadToEnd();

        Logger.Warn("Shockeline/ShockClient", controlResponse.ToString());
        Logger.Warn("Shockeline/ShockClient", controlResponse.StatusCode.ToString());
        Logger.Warn("Shockeline/ShockClient", res);
        Logger.Warn("Shockeline/ShockClient", controlResponse.RequestMessage.ToString());
        Logger.Warn("Shockeline/ShockClient", sent);
        Logger.Warn("Shockeline/ShockClient", controlResponse.RequestMessage.RequestUri.ToString());
        Logger.Warn("Shockeline/ShockClient", controlResponse.RequestMessage.Headers.ToString());

        if (controlResponse.IsSuccessStatusCode) return true;

        return false;
    }

    public class ApiOptions
    {
        public Uri Server { get; set; } = new("https://api.openshock.app");
        public required string Token { get; set; }
    }
}