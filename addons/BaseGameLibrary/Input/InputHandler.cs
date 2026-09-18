using Godot;

namespace ElementGodot.BaseGameLibrary.Input;

[GlobalClass]
public partial class InputHandler : Resource
{
    private System.Collections.Generic.Dictionary<string, PlayerInputDelegate> _delegatesRegistereds = new();

    public void _Register(string p_sName, DInputPressed p_delegate)
    {
        if (_delegatesRegistereds.TryGetValue(p_sName, out PlayerInputDelegate? sPlayerInputDelegate))
        {
            sPlayerInputDelegate.On_Pressed += p_delegate;
        }
        else
        {
            PlayerInputDelegate sData = new(p_sName);
            sData.On_Pressed += p_delegate;
            _delegatesRegistereds.Add(p_sName, sData);
        }
    }

    public void _Unregister(string p_sName, DInputPressed p_delegate)
    {
        if (_delegatesRegistereds.TryGetValue(p_sName, out PlayerInputDelegate? sPlayerInputDelegate))
            sPlayerInputDelegate.On_Pressed -= p_delegate;
    }

    public virtual void _ShortcutInput(InputEvent p_input, Viewport p_viewport)
    {
    }

    public virtual void _UnhandledInput(InputEvent p_input, Viewport p_viewport)
    {
        if (!p_input.IsPressed())
            foreach (string sRegisteredAction in _delegatesRegistereds.Keys)
                if (p_input.IsAction(sRegisteredAction) && _delegatesRegistereds.TryGetValue(sRegisteredAction,
                        out PlayerInputDelegate? sPlayerInputDelegate))
                {
                    sPlayerInputDelegate._Trigger(p_input);
                    p_viewport.SetInputAsHandled();
                }
    }
    
    public virtual void _KeyBoardInput(InputEvent p_input, Viewport p_viewport)
    {
    }
}
