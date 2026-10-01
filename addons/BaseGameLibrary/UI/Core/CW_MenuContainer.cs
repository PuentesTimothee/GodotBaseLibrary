
using System.Linq;
using ElementGodot.Tags;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.UI.Core;

[GlobalClass, Icon("res://addons/BaseGameLibrary/editor/Ic_Menu.svg")] [Tool]
public partial class CW_MenuContainer : CW_ActivatableContainer
{
    protected Tag _TagInternal => Tag.Invalid();
    public virtual Tag GetTag() => _TagInternal;
    
    [Export] public bool _Closeable = true;
    [Export] public StringName _MenuName = "!!Invalid!!";
    public CW_Header? _Header = null;
    public CW_Footer? _Footer = null;

    public void _OpenOtherMenu(Tag p_sTag) => MainSceneBase.MenuManager._OpenMenu(p_sTag);
    public void _OpenOtherMenuFromString(string p_sTag) => _OpenOtherMenu(Tag.RequestTag(p_sTag));
    
    public override string[] _GetConfigurationWarnings()
    {
        Array<string> sData = new();
        if (GetTag() is null || !GetTag().IsValid())
            sData.Add("Must initialize property '_LinkedTag'.");
        return sData.ToArray();
    }
}