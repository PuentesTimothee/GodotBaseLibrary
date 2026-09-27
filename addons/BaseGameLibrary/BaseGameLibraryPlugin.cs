#if TOOLS
using ElementGodot.BaseGameLibrary.Datas;
using ElementGodot.BaseGameLibrary.editor;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary;

[Tool]
public partial class BaseGameLibraryPlugin : BasePlugin
{
	public readonly string _AutoloadName_GdManager = "Instance_GameDatasManager";
	public readonly string _AutoloadName_GameSettings = "Instance_GameSettings";

	public override void _EnterTree()
	{
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
