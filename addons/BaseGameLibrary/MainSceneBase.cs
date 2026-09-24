using System;
using System.IO;
using ElementGodot.BaseGameLibrary.Gameplay;
using ElementGodot.BaseGameLibrary.Helpers;
using ElementGodot.BaseGameLibrary.Input;
using ElementGodot.BaseGameLibrary.UI.Core;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary;

public partial class MainSceneBase : Node3D
{
	protected static MainSceneBase? HiddenInstance = null;
	
	protected GameModeBase? _InternalGameMode = null;

	[Export] protected Camera3D? _InternalCamera3D = null;
	protected BaseInputNode _InputNode { get; set; }
	
	[Export] protected PackedScene? _MenuManagerPacked;
	protected CW_MenuManager _InternalMenusManager = null!;

	public static RandomNumberGenerator RandomNumberGenerator = null!;

	public static BaseInputNode InputNode => MainSceneBase.HiddenInstance?._InputNode!;
	public static CW_MenuManager MenuManager => MainSceneBase.HiddenInstance?._InternalMenusManager!;
	public static GameModeBase GetNakedMode() => MainSceneBase.HiddenInstance?._InternalGameMode!;
	protected static TGamemode? Private_GetModeCanFail<TGamemode>() where TGamemode : GameModeBase => MainSceneBase.GetNakedMode() as TGamemode;
	protected static TGamemode Private_GetMode<TGamemode>() where TGamemode : GameModeBase => MainSceneBase.Private_GetModeCanFail<TGamemode>() ?? throw new InvalidDataException($"No gamemode found {nameof(TGamemode)}.");

	public static bool IsTestMode = true;
	
	// Warn users if the value hasn't been set.
	public override string[] _GetConfigurationWarnings()
	{
		if (_MenuManagerPacked == null && _InternalCamera3D == null)
			return ["Must initialize property '_MenuManagerPacked' & '_InternalCamera3D'."];
		if (_MenuManagerPacked == null)
			return ["Must initialize property '_MenuManagerPacked'."];
		if (_InternalCamera3D == null)
			return ["Must initialize property '_InternalCamera3D'."];
		return [];
	}
	
	public MainSceneBase()
	{
		MainSceneBase.HiddenInstance = this;
		MainSceneBase.RandomNumberGenerator = new RandomNumberGenerator();
		
		var args = new Array<string>(OS.GetCmdlineUserArgs());
		MainSceneBase.IsTestMode = args.Contains("--testMode");

		Dictionary<string, LogType> sValueString = new();
		string sSeedString = "--seed=";
		sValueString.Add("--log=", LogType.e_Default);
		sValueString.Add("--logW=", LogType.e_Warning);
		sValueString.Add("--logE=", LogType.e_Error);
		
		foreach (string arg in args)
		{
			foreach (var (str, val) in sValueString)
			{
				if (arg.StartsWith(str))
					if (TimEnums.TryParseWithPrefix(arg.Substring(str.Length + 1), out LogSeverity logType))
						MyLogger._SetSeverityFor(val, logType);
			}

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
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_InternalMenusManager.QueueFree();
	}
}
