using System;
using System.Diagnostics;
using System.Linq;
using BaseGameLibrary.Helpers;
using BaseGameLibrary.Settings;
using Tags;
using Godot;
using Godot.Collections;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary.UI.Settings;

[GlobalClass, Tool]
public partial class CW_SettingEntry : PanelContainer
{
    [Export] public Tag _CurrentTag = null!;
    [Export, NodeNeeded] public RichTextLabel _Title = null!;
    [Export, NodeNeeded] public Control _Control = null!;
    public Control _SpawnedControl = null!;

    protected GameSingleSetting? _LinkedSetting;
    
    public override void _Ready()
    {
        base._Ready();

        _LinkedSetting = GameSettings.Instance.GetSetting<GameSingleSetting>(_CurrentTag);
        Debug.Assert(_LinkedSetting is null, nameof(CW_SettingEntry._LinkedSetting) + $" Is null [${_CurrentTag}]");

        _Title.SetText(_CurrentTag.FullName());
        _Control.AddChild(_SpawnedControl = _LinkedSetting!.MakeControl());
    }
}
