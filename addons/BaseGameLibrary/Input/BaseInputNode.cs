using System.Linq;
using BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;
using Tags;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary.Input;

public delegate void DInputPressed(InputEvent p_inputEvent);

public record PlayerInputDelegate(Tag _InputTag)
{
    public event DInputPressed? On_Pressed = null;

    public readonly StringName _LinkedAction = TimEnums._StripPrefix(_InputTag._StringTag, "input.");

    public void _Trigger(InputEvent p_inputEvent) => On_Pressed?.Invoke(p_inputEvent);
}

[GlobalClass]
public partial class BaseInputNode : Node
{
    private Array<InputHandler> _inputHandlers = new();
    private IOrderedEnumerable<InputHandler>? _orderedInputHandler;

    public void UpdateOrderedInputHandlers() => _orderedInputHandler = _inputHandlers.OrderBy(p_handler => p_handler._Priority);
    public void AddNewInputHandler(InputHandler p_inputHandler)
    {
        if (p_inputHandler._OnlyInTest && !MainSceneBase.IsTestMode)
            return;
        
        if (!_inputHandlers.Contains(p_inputHandler))
        {
            _inputHandlers.Add(p_inputHandler);
            UpdateOrderedInputHandlers();
        }
    }

    public void RemoveInputHandler(InputHandler p_inputHandler)
    {
        if (_inputHandlers.Remove(p_inputHandler))
            UpdateOrderedInputHandlers();
    }

    public override void _ShortcutInput(InputEvent p_input) => _orderedInputHandler?._ForEach(p_sHandler => p_sHandler._ShortcutInput(p_input, GetViewport()));
    public override void _UnhandledInput(InputEvent p_input) => _orderedInputHandler?._ForEach(p_sHandler => p_sHandler._UnhandledInput(p_input, GetViewport()));
    public override void _UnhandledKeyInput(InputEvent p_input) => _orderedInputHandler?._ForEach(p_sHandler => p_sHandler._UnhandledInput(p_input, GetViewport()));
}
