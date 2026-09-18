#if TOOLS
using ElementGodot.BaseGameLibrary.Tags.editor;
using Godot;

namespace ElementGodot.BaseGameLibrary.Tags;

[Tool]
public partial class TagEditor : EditorPlugin
{
	public readonly string _AutoloadName_Tags = "Instance_TagsManager";

	private TagsEditorDock? _tagsEditorDock;
	private TagContainerInspectorPlugin? _tagContainerInspectorPlugin;
	private QueryExpressionInspectorPlugin? _queryExpressionInspectorPlugin;
	private TagInspectorPlugin? _tagInspectorPlugin;

	private EditorFileSystem? _fileSystem;

	public override void _EnablePlugin()
	{
		AddAutoloadSingleton(_AutoloadName_Tags, "res://addons/Tags/TagsManager.cs");
	}

	public override void _DisablePlugin()
	{
		RemoveAutoloadSingleton(_AutoloadName_Tags);
	}

	public override void _EnterTree()
	{
		
		_tagsEditorDock = new TagsEditorDock();
		AddDock(_tagsEditorDock);
		_tagContainerInspectorPlugin = new TagContainerInspectorPlugin();
		AddInspectorPlugin(_tagContainerInspectorPlugin);
		_queryExpressionInspectorPlugin = new QueryExpressionInspectorPlugin();
		AddInspectorPlugin(_queryExpressionInspectorPlugin);
		_tagInspectorPlugin = new TagInspectorPlugin();
		AddInspectorPlugin(_tagInspectorPlugin);

		AddToolMenuItem("Repair assets tags", new Callable(this, MethodName.CallAssetRepairTool));
	}

	public override void _ExitTree()
	{
		if (_tagsEditorDock is not null)
		{
			RemoveDock(_tagsEditorDock);
			_tagsEditorDock.Free();
			_tagsEditorDock = null;
		}

		RemoveInspectorPluginAndRelease(ref _tagContainerInspectorPlugin);
		RemoveInspectorPluginAndRelease(ref _queryExpressionInspectorPlugin);
		RemoveInspectorPluginAndRelease(ref _tagInspectorPlugin);

		_fileSystem = null;

		RemoveToolMenuItem("Repair assets tags");
	}


	public override string _GetPluginName() => "TagEditor";

	private static void CallAssetRepairTool() => AssetRepairTool.RepairAllAssetsTags();

	private void RemoveInspectorPluginAndRelease<TPlugin>(ref TPlugin? plugin)
		where TPlugin : EditorInspectorPlugin
	{
		if (plugin is null)
			return;

		RemoveInspectorPlugin(plugin);
		plugin = null;
	}
}
#endif
