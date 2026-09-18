using System;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Settings;

public partial class GameSingleSettings_Locale : GameSingleSetting_Dropdown
{
    public GameSingleSettings_Locale() : base(ESettingsSection.e_Game)
    {
    }
    
    public override void Init() => _PossiblesValues = new Array<string>(TranslationServer.GetLoadedLocales());
    public override void LoadValueFromState() => SetValue(TranslationServer.GetLocale(), false);
    public override void OnValueChanged(string? p_value, string? _) => TranslationServer.SetLocale(p_value);
}

public partial class GameSingleSettings_Resolution : GameSingleSetting_Dropdown
{
    public GameSingleSettings_Resolution() : base(ESettingsSection.e_Graphic)
    {
    }

    public override void Init() => _PossiblesValues = new Array<string>(TranslationServer.GetLoadedLocales());
    public override void LoadValueFromState() => SetValue(TranslationServer.GetLocale(), false);
    public override void OnValueChanged(string? p_value, string? _) => TranslationServer.SetLocale(p_value);
}

public partial class GameSingleSettings_DisplayType : GameSingleSetting_Dropdown
{
    public GameSingleSettings_DisplayType() : base(ESettingsSection.e_Graphic)
    {
    }

    public override void Init() => _PossiblesValues = new Array<string>(Enum.GetNames<DisplayServer.WindowMode>());
    public override void LoadValueFromState() => SetValue(Enum.GetName(DisplayServer.WindowGetMode()) ?? string.Empty, false);
    public override void OnValueChanged(string? p_value, string? _) => DisplayServer.WindowSetMode(Enum.Parse<DisplayServer.WindowMode>(p_value!));
}

public partial class GameSingleSettings_VSync : GameSingleSetting_Dropdown
{
    public GameSingleSettings_VSync() : base(ESettingsSection.e_Graphic)
    {
    }
    
    public override void Init() => _PossiblesValues = new Array<string>(Enum.GetNames<DisplayServer.VSyncMode>());
    public override void LoadValueFromState() => SetValue(DisplayServer.WindowGetVsyncMode().ToString(), false);
    public override void OnValueChanged(string? p_value, string? _) => DisplayServer.WindowSetVsyncMode(Enum.Parse<DisplayServer.VSyncMode>(p_value!));
}