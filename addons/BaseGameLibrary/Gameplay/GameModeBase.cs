using Godot;

namespace ElementGodot.BaseGameLibrary.Gameplay;

public abstract partial class GameModeBase : Node
{
	public GameModeBase()
	{
	}

	public virtual void _GameModeBegin()
	{
	}

	public void _FinishedLoading() => EmitSignalOnGamemodeReady(this);

	[Signal]
	public delegate void OnGamemodeReadyEventHandler(GameModeBase p_gm);
}
