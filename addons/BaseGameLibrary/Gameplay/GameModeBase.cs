using Godot;

namespace ElementGodot.BaseGameLibrary.Gameplay;

/// <summary>
/// <para> Basic Building block for your game. Used to Compartmentalize the Game logics</para>
/// </summary>
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
