using System;
using Godot;

namespace ElementGodot.BaseGameLibrary.Input;

public delegate void DInputPressed(InputEvent p_inputEvent);

public record PlayerInputDelegate(string _InputString)
{
	public event DInputPressed? On_Pressed = null;

	public void _Trigger(InputEvent p_inputEvent) => On_Pressed?.Invoke(p_inputEvent);
}

[GlobalClass]
public partial class BaseInputNode : Node
{
	private InputHandler? _inputHandler = new InputHandler();

	public void SetNewInputHandler(InputHandler p_inputHandler) => _inputHandler = p_inputHandler;
	
	public void _Register(EInputList p_eInputList, DInputPressed p_delegate)
	{
		string? sName = Enum.GetName(typeof(EInputList), p_eInputList);
		if (sName is not null)
			_Register(sName, p_delegate);
	}

	public void _Register(string p_sName, DInputPressed p_delegate) => _inputHandler?._Register(p_sName, p_delegate);

	public void _Unregister(EInputList p_eInputList, DInputPressed p_delegate)
	{
		string? sName = Enum.GetName(typeof(EInputList), p_eInputList);
		if (sName is not null)
			_Unregister(sName, p_delegate);
	}

	public void _Unregister(string p_sName, DInputPressed p_delegate) => _inputHandler?._Unregister(p_sName, p_delegate);


	public override void _ShortcutInput(InputEvent p_input) => _inputHandler?._ShortcutInput(p_input, GetViewport());
	public override void _UnhandledInput(InputEvent p_input) => _inputHandler?._UnhandledInput(p_input, GetViewport());
	public override void _UnhandledKeyInput(InputEvent p_input) => _inputHandler?._KeyBoardInput(p_input, GetViewport());
}
