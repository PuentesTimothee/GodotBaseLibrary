using System.Linq;
using System.Reflection;
using BaseGameLibrary.Helpers;
using Godot;
using GameModeBase = BaseGameLibrary.Gameplay.GameModeBase;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary.UI.Core;

[GlobalClass]
public abstract partial class CW_Control : Control
{
	public override string[] _GetConfigurationWarnings() =>
		ReflectionHelper._GetConfigurationWarnings<Control, NodeNeededAttribute>(this);
	
	public override void _Ready()
	{
		base._Ready();
		MainSceneBase.GetNakedMode().OnGamemodeReady += _OnGameModeReady;
	}

	protected virtual void _OnGameModeReady(GameModeBase p_gm)
	{
	}
}
