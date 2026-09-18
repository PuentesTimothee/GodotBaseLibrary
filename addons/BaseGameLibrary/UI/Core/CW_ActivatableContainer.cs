using System;
using Godot;

namespace ElementGodot.BaseGameLibrary.UI.Core;

public partial class CW_ActivatableContainer : PanelContainer
{
    [Export] public bool _IsActivated { private set; get; } = false;

    public event Action<CW_ActivatableContainer>? On_Activated;
    public event Action<CW_ActivatableContainer>? On_Deactivated;

    public override void _Ready()
    {
        SetVisible(_IsActivated);
    }

    public void _SetActivated(bool p_bNewState)
    {
        if (p_bNewState)
            _Activate();
        else
            _Deactivate();
    }

    public void _ToggleState()
    {
        if (_IsActivated)
            _Deactivate();
        else
            _Activate();
    }

    public void _Activate()
    {
        if (_IsActivated)
            return;

        _IsActivated = true;
        _UpdateVisible();
        On_Activated?.Invoke(this);
        _OnActivate();
    }
    public virtual void _OnActivate() {}

    public void _Deactivate()
    {
        if (!_IsActivated)
            return;

        _IsActivated = false;
        _UpdateVisible();
        On_Deactivated?.Invoke(this);
        _OnDeactivate();
    }
    public virtual void _OnDeactivate() {}

    public void _UpdateVisible()
    {
        SetVisible(_IsActivated);
    }
}