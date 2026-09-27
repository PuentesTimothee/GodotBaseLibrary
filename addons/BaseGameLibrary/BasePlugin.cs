#if TOOLS
using ElementGodot.BaseGameLibrary.editor;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary;

[Tool]
public partial class BasePlugin : EditorPlugin
{
	private Array<EditorInspectorPlugin> _tagInspectorPlugins = new();

	public override void _ExitTree()
	{
		foreach (EditorInspectorPlugin plugin in _tagInspectorPlugins)
		{
			RemoveInspectorPlugin(plugin);
			plugin.Free();
		}
		_tagInspectorPlugins.Clear();
	}

	protected void AddCustomInspectorPlugin<TPlugin>()
		where TPlugin : EditorInspectorPlugin, new()
	{
		TPlugin sData = new TPlugin();
		
		AddInspectorPlugin(sData);
		_tagInspectorPlugins.Add(sData);
	}
}
#endif
