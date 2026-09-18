using System.IO;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;

namespace ElementGodot.BaseGameLibrary.UI.Core;

[GlobalClass, Tool]
public partial class CW_Footer : MarginContainer
{
    protected CW_MenuContainer? _Owner = null!;
    
    public CW_Footer()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        _LinkToMenu(this.FindParentOfType<CW_MenuContainer>());
    }

    private void OnCloseButtonPressed()
    {
    }

    public void _LinkToMenu(CW_MenuContainer? p_owner)
    {
        if (p_owner is null)
            return;
        
        _Owner = p_owner;

        if (_Owner._Footer is not null && _Owner._Footer != this)
            throw new InvalidDataException($"Duplicate Footer for menu {_Owner._LinkedTag}");
        _Owner._Footer = this;
    }
}