
using System.Linq;
using ElementGodot.Tags;
using Godot;
using Godot.Collections;
using Tag = ElementGodot.Tags.Tag;

namespace ElementGodot.BaseGameLibrary.UI.Core;

[GlobalClass, Tool]
public partial class CW_MenuContainer : CW_ActivatableContainer
{
    [Export] protected TagRessource? _LinkedTagRessource ;
    protected Tag _LinkedTag = Tag.Invalid();
    public Tag _Tag => _LinkedTagRessource is not null ? _LinkedTagRessource.GetTag() : _LinkedTag;
    
    [Export] public bool _Closeable = true;
    [Export] public StringName _MenuName = "!!Invalid!!";
    public CW_Header? _Header = null;
    public CW_Footer? _Footer = null;

    public void _OpenOtherMenu(Tag p_sTag) => MainSceneBase.MenuManager._OpenMenu(p_sTag);
    public void _OpenOtherMenuFromString(string p_sTag) => _OpenOtherMenu(Tag.RequestTag(p_sTag));
    
    public override string[] _GetConfigurationWarnings()
    {
        Array<string> sData = new();
        if (!_LinkedTag.IsValid() && _LinkedTagRessource is null)
            sData.Add("Must initialize property '_LinkedTag'.");
        return sData.ToArray();
    }
    
    
}