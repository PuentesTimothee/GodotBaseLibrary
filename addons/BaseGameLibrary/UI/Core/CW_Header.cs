using System.IO;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;

namespace ElementGodot.BaseGameLibrary.UI.Core;

[GlobalClass, Tool]
public partial class CW_Header : MarginContainer
{
    protected CW_MenuContainer? _Owner = null!;
    public RichTextLabel? _MenuName;
    public Button? _CloseButton;
    
    public CW_Header()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        _LinkToMenu(this.FindParentOfType<CW_MenuContainer>());
        
        if (_CloseButton is not null)
            _CloseButton.Pressed += OnCloseButtonPressed;
    }

    private void OnCloseButtonPressed()
    {
        if (_Owner is not null && _Owner._Closeable)
            _Owner._Deactivate();
    }

    public void _LinkToMenu(CW_MenuContainer? p_owner)
    {
        if (p_owner is null)
            return;
        
        _Owner = p_owner;
        
        if (_Owner._Header is not null && _Owner._Header != this)
            throw new InvalidDataException($"Duplicate Header for menu {_Owner._Tag}");

        _Owner._Header = this;
        _MenuName?.SetText(p_owner._MenuName);
        _CloseButton?.SetVisible(p_owner._Closeable);
    }
}