#if TOOLS
using BaseGameLibrary.Datas;
using BaseGameLibrary.editor;
using BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary;

[Tool]
public partial class BaseGameLibraryPlugin : BasePlugin
{
	public readonly string _AutoloadName_GdManager = "Instance_GameDatasManager";
	public readonly string _AutoloadName_GameSettings = "Instance_GameSettings";

	public GameDatasManager _GameDatasManager = new GameDatasManager(); 
	public override void _EnterTree()
	{	
		AddChild(_GameDatasManager = new GameDatasManager());
		AddCustomInspectorPlugin<MenuContainerInspectorPlugin>();
	}

	public override void _EnablePlugin()
	{
		AddAutoloadSingleton(_AutoloadName_GdManager,"res://addons/BaseGameLibrary/Datas/GameDatasManager.cs");
		AddAutoloadSingleton(_AutoloadName_GameSettings, "res://addons/BaseGameLibrary/Settings/GameSettings.cs");
	}

	public override void _DisablePlugin() 
	{
		RemoveAutoloadSingleton(_AutoloadName_GdManager);
		RemoveAutoloadSingleton(_AutoloadName_GameSettings);
	}
}
#endif
