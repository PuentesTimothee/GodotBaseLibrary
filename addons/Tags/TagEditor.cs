#if TOOLS
using ElementGodot.BaseGameLibrary;
using ElementGodot.BaseGameLibrary.editor;
using ElementGodot.Tags.editor;
using Godot;

namespace ElementGodot.Tags;

[Tool]
public partial class TagEditor : BasePlugin
{
	public readonly string _AutoloadName_Tags = "Instance_TagsManager";

	private TagsEditorDock? _tagsEditorDock;
	private MenuContainerInspectorPlugin? _tagContainerInspectorPlugin;
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
		
		AddCustomInspectorPlugin<TagContainerInspectorPlugin>();
		AddCustomInspectorPlugin<QueryExpressionInspectorPlugin>();
		AddCustomInspectorPlugin<TagInspectorPlugin>();

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

		_fileSystem = null;

		RemoveToolMenuItem("Repair assets tags");
	}


	public override string _GetPluginName() => "TagEditor";

	private static void CallAssetRepairTool() => AssetRepairTool.RepairAllAssetsTags();
}
#endif
