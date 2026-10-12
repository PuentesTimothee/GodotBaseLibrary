using System.Collections.Generic;
using BaseGameLibrary.Helpers;
using Godot;
using Tags;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary.Input;

[GlobalClass]
public partial class InputHandler : Resource
{
    public static readonly int PrioritySystem = 0;
    public static readonly int PriorityUi = 100;
    public static readonly int PriorityGameplay = 200;
    public static readonly int PriorityNonSet = 9999;
    
    public bool _EnableDuringPause = false;
    public int _Priority = PriorityNonSet;
    public readonly bool _OnlyInTest = false;
    
    public const string TAG_PREFIX = "input.";
    public static StringName GetFromTag(Tag p_tag) => TimEnums._StripPrefix(p_tag._StringTag, InputHandler.TAG_PREFIX);
    
    private Dictionary<StringName, PlayerInputDelegate> _delegatesRegistereds = new();

    public InputHandler() => _OnlyInTest = false;
    public InputHandler(bool p_bTest = false) => _OnlyInTest = p_bTest;

    public void _Register(Tag p_tag, DInputPressed p_delegate)
    {
        StringName sName = GetFromTag(p_tag);
        
        if (_delegatesRegistereds.TryGetValue(sName, out PlayerInputDelegate? sPlayerInputDelegate))
            sPlayerInputDelegate.On_Pressed += p_delegate;
        else
        {
            PlayerInputDelegate sData = new(p_tag);
            sData.On_Pressed += p_delegate;
            _delegatesRegistereds.Add(sName, sData);
        }
    }

    public void _Unregister(StringName p_sName, DInputPressed p_delegate)
    {
        if (_delegatesRegistereds.TryGetValue(p_sName, out PlayerInputDelegate? sPlayerInputDelegate))
            sPlayerInputDelegate.On_Pressed -= p_delegate;
    }

    public bool IsHandlerDisabled(Viewport p_viewport) =>
        p_viewport.IsInputHandled() || (!_EnableDuringPause && MainSceneBase.IsPaused);
    
    public void _ShortcutInput(InputEvent p_input, Viewport p_viewport)
    {
        if (IsHandlerDisabled(p_viewport))
            return;
        _ShortcutInput_Impl(p_input, p_viewport);
    }
    
    public virtual void _ShortcutInput_Impl(InputEvent p_input, Viewport p_viewport)
    {
    }
    
    public virtual void _UnhandledInput(InputEvent p_input, Viewport p_viewport)
    {
        if (IsHandlerDisabled(p_viewport))
            return;
        
        if (!p_input.IsPressed())
            foreach (var (sRegisteredAction, sPlayerInputDelegate) in _delegatesRegistereds)
                if (p_input.IsAction(sRegisteredAction))
                {
                    sPlayerInputDelegate._Trigger(p_input);
                    p_viewport.SetInputAsHandled();
                }
    }
    
    public virtual void _KeyBoardInput(InputEvent p_input, Viewport p_viewport)
    {
        if (IsHandlerDisabled(p_viewport))
            return;
        _KeyBoardInput_Impl(p_input, p_viewport);
    }

    public virtual void _KeyBoardInput_Impl(InputEvent p_input, Viewport p_viewport)
    {
    }
}
