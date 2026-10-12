using System;
using System.IO;
using BaseGameLibrary.Gameplay;
using BaseGameLibrary.Helpers;
using BaseGameLibrary.Input;
using BaseGameLibrary.UI;
using BaseGameLibrary.UI.Core;
using Godot;
using Godot.Collections;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary;

public partial class MainSceneBase : Node3D
{
	protected static MainSceneBase? HiddenInstance = null;
	
	protected GameModeBase? _InternalGameMode = null;

	[Export, NodeNeeded] protected Camera3D? _InternalCamera3D = null;
	
	[Export, NodeNeeded] protected PackedScene? _MenuManagerPacked;
	protected CW_MenuManager _InternalMenusManager = null!;

	public static RandomNumberGenerator RandomNumberGenerator = null!;
	public static CW_MenuManager MenuManager => MainSceneBase.HiddenInstance?._InternalMenusManager!;
	public static GameModeBase GetNakedMode() => MainSceneBase.HiddenInstance?._InternalGameMode!;
	protected static TGamemode? Private_GetModeCanFail<TGamemode>() where TGamemode : GameModeBase => MainSceneBase.GetNakedMode() as TGamemode;
	protected static TGamemode Private_GetMode<TGamemode>() where TGamemode : GameModeBase => MainSceneBase.Private_GetModeCanFail<TGamemode>() ?? throw new InvalidDataException($"No gamemode found {nameof(TGamemode)}.");

	public static bool IsTestMode = true;

	protected BaseInputNode _InputNode { get; set; }

	//
	// Input section
	protected InputHandler _DebugInputHandler = new InputHandler(true)
	{
		_Priority = InputHandler.PrioritySystem
	};
	
	public static BaseInputNode InputNode => MainSceneBase.HiddenInstance?._InputNode!;
	// !Input section
	//
	
	// Warn users if the value hasn't been set.
	public override string[] _GetConfigurationWarnings() =>
		ReflectionHelper._GetConfigurationWarnings<Node, NodeNeededAttribute>(this);
	
	public MainSceneBase()
	{
		MainSceneBase.HiddenInstance = this;
		MainSceneBase.RandomNumberGenerator = new RandomNumberGenerator();
		
		var args = new Array<string>(OS.GetCmdlineUserArgs());
		MainSceneBase.IsTestMode = args.Contains("--testMode");

		string sSeedString = "--seed=";
		foreach (string arg in args)
		{
			if (arg.StartsWith(sSeedString))
				MainSceneBase.RandomNumberGenerator.Seed = ulong.Parse(arg.Substring(sSeedString.Length + 1));
		}
		
		AddChild(_InputNode ??= new());
	}

	public override void _Ready()
	{
		base._Ready();
		
		_InternalMenusManager = _MenuManagerPacked?.Instantiate<CW_MenuManager>() ?? throw new InvalidOperationException($"No MenuManager found.");
		AddChild(_InternalMenusManager);

		_InternalGameMode?._GameModeBegin();
		_InternalGameMode?._FinishedLoading();
		
		
		MainSceneBase.InputNode.AddNewInputHandler(_DebugInputHandler);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_InternalMenusManager.QueueFree();
		
		MainSceneBase.InputNode.RemoveInputHandler(_DebugInputHandler);
	}

	public static bool IsPaused => MainSceneBase.HiddenInstance!.GetTree().Paused;

	public virtual void _SetPause(bool p_bState)
	{
		GetTree().Paused = p_bState;
	}
}
