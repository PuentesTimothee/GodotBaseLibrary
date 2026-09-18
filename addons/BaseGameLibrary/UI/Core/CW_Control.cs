using Godot;
using GameModeBase = ElementGodot.BaseGameLibrary.Gameplay.GameModeBase;

namespace ElementGodot.BaseGameLibrary.UI.Core;

[GlobalClass]
public abstract partial class CW_Control : Control
{
	public override void _Ready()
	{
		base._Ready();
		MainSceneBase.GetNakedMode().OnGamemodeReady += _OnGameModeReady;
	}

	protected virtual void _OnGameModeReady(GameModeBase p_gm)
	{
	}
}
