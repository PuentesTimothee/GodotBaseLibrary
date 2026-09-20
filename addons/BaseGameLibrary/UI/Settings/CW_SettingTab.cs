using ElementGodot.BaseGameLibrary.Settings;
using ElementGodot.BaseGameLibrary.Tags;
using ElementGodot.BaseGameLibrary.UI.Core;
using Godot;
using Godot.Collections;

namespace ElementGodot.Scripts.UI.Settings;

[GlobalClass]
public partial class CW_SettingTab : CW_ActivatableContainer
{
    [Export] public Array<Tag> _AllLine = new();
    [Export] public CW_ListObjectContainer _Container = null!;
    
    public void InitAllOptions()
    {
        GameSettings pSetting = GameSettings.Instance;
        foreach (GodotObject sObject in _AllLine)
        {
        }
    }
}