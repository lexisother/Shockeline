using System;
using Celeste.Mod.UI;
using YamlDotNet.Serialization;

namespace Celeste.Mod.Shockeline;

public class ShockelineModuleSettings : EverestModuleSettings
{
    public enum ControlType
    {
        Shock = 0,
        Rumble = 1
    }

    // Regular options
    public ControlType Type { get; set; } = ControlType.Shock;

    [SettingRange(1, 100)] public int Intensity { get; set; } = 5;
    [SettingRange(300, 30000, true)] public int Duration { get; set; } = 500;
    [SettingMaxLength(36)] public string DeviceId { get; set; } = "";
    [SettingMaxLength(64)] public string Token { get; set; } = "";


    #region Custom UI

    [SettingIgnore] [YamlIgnore] public TextMenu.Button? TokenEntry { get; protected set; }

    public void CreateTokenEntry(TextMenu menu, bool inGame)
    {
        TokenEntry = CreateMenuStringInput(menu, "Token", s => Token.Length > 0 ? "Key: ###HIDDEN###" : "Key: -", 64,
            () => Token,
            newVal => Token = newVal);
    }

    public TextMenu.Button CreateMenuStringInput(TextMenu menu, string label, Func<string, string> dialogTransform,
        int maxValueLength, Func<string> currentValue, Action<string> newValue)
    {
        var item = new TextMenu.Button(dialogTransform?.Invoke(label) ?? label);
        item.Pressed(() =>
        {
            Audio.Play("event:/ui/main/savefile_rename_start");
            menu.SceneAs<Overworld>().Goto<OuiModOptionString>().Init<OuiModOptions>(
                currentValue.Invoke(),
                v => newValue.Invoke(v),
                maxValueLength
            );
        });
        menu.Add(item);
        return item;
    }

    #endregion
}