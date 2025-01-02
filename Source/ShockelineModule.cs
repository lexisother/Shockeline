using System;

namespace Celeste.Mod.Shockeline;

public class ShockelineModule : EverestModule
{
    public ShockelineModule()
    {
        Instance = this;

#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(ShockelineModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(ShockelineModule), LogLevel.Info);
#endif
    }

    public static ShockClient? ShockClient { get; private set; }

    public static ShockelineModule Instance { get; private set; }

    public override Type SettingsType => typeof(ShockelineModuleSettings);
    public static ShockelineModuleSettings Settings => (ShockelineModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(ShockelineModuleSession);
    public static ShockelineModuleSession Session => (ShockelineModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(ShockelineModuleSaveData);
    public static ShockelineModuleSaveData SaveData => (ShockelineModuleSaveData)Instance._SaveData;

    public override void Load()
    {
        Everest.Events.Player.OnDie += Shock;
    }

    public override void Unload()
    {
        Everest.Events.Player.OnDie -= Shock;
    }

    public ShockClient? GetClient()
    {
        if (Settings.Token.Length > 0 && Settings.DeviceId.Length > 0)
        {
            if (ShockClient != null) return ShockClient;

            return ShockClient = new ShockClient(new ShockClient.ApiOptions
            {
                Token = Settings.Token
            });
        }

        return null;
    }

    private async void Shock(Player player)
    {
        var client = GetClient();
        if (client == null) return;

        var res = await ShockClient.ControlShocker(new Guid(Settings.DeviceId));
        Logger.Warn("Shockeline/Shock", res.ToString());
    }
}