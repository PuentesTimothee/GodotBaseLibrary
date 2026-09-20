
using ElementGodot.BaseGameLibrary.Helpers;
using System.Diagnostics;
using System.Linq;
using ElementGodot.BaseGameLibrary.Settings;
using ElementGodot.BaseGameLibrary.Tags;
using Godot;
using Godot.Collections;

namespace ElementGodot.Scripts.UI.Settings;

[GlobalClass]
public partial class CW_SettingEntry : PanelContainer
{
    [Export] public Tag _CurrentTag = null!;
    [Export] public RichTextLabel _Title = null!;
    [Export] public Control _Control = null!;
    public Control _SpawnedControl = null!;

    protected GameSingleSetting? _LinkedSetting;
    
    public override string[] _GetConfigurationWarnings()
    {
        Array<string> sData = new();
        if (_Title == null)
            sData.Add("Must initialize property '_Title'.");
        if (_Control == null)
            sData.Add("Must initialize property '_Slot'.");
        return sData.ToArray();
    }
    
    public override void _Ready()
    {
        base._Ready();
        
        _LinkedSetting = GameSettings.Instance.GetSetting<GameSingleSetting>(_CurrentTag);
        Debug.Assert(_LinkedSetting is null, nameof(CW_SettingEntry._LinkedSetting) + $" Is null [${_CurrentTag._StringTag}]");

        _Title.SetText(_CurrentTag._StringTag);
        _Control.AddChild(_SpawnedControl = _LinkedSetting!.MakeControl());
    }
}