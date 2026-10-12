using BaseGameLibrary.Settings;
using BaseGameLibrary.UI.Core;
using Godot;
using Godot.Collections;
using Tags;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary.UI.Settings;

[GlobalClass, Tool]
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
