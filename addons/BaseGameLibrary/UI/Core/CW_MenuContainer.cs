
using System.Linq;
using Godot;
using Godot.Collections;
using Tag = ElementGodot.BaseGameLibrary.Tags.Tag;

namespace ElementGodot.BaseGameLibrary.UI.Core;

[GlobalClass, Tool]
public partial class CW_MenuContainer : CW_ActivatableContainer
{
    [Export] public Tag _LinkedTag = Tag.Invalid();
    [Export] public bool _Closeable = true;
    [Export] public StringName _MenuName = "!!Invalid!!";
    public CW_Header? _Header = null;
    public CW_Footer? _Footer = null;

    public void _OpenOtherMenu(Tag p_sTag) => MainSceneBase.MenuManager._OpenMenu(p_sTag);
    public void _OpenOtherMenuFromString(string p_sTag) => _OpenOtherMenu(Tag.RequestTag(p_sTag));
    
    public override string[] _GetConfigurationWarnings()
    {
        Array<string> sData = new();
        if (_LinkedTag == null || !_LinkedTag.IsValid())
            sData.Add("Must initialize property '_LinkedTag'.");
        return sData.ToArray();
    }
    
    
}