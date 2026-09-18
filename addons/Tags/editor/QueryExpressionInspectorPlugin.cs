// Copyright © Gamesmiths Guild.

#if TOOLS
using Godot;

namespace ElementGodot.BaseGameLibrary.Tags.editor;

[Tool]
public partial class QueryExpressionInspectorPlugin : EditorInspectorPlugin
{
	public override bool _CanHandle(GodotObject p_object) => p_object is TagQuery;

	public override void _ParseBegin(GodotObject p_object)
	{
		if (p_object is not TagQuery query)
			return;

		var editor = new QueryExpressionEditorControl();
		editor.Setup(query, query.EmitChanged);
		AddCustomControl(editor);
	}

	public override bool _ParseProperty(GodotObject p_object, Variant.Type p_type, string p_name, PropertyHint p_hintType, string p_hintString, PropertyUsageFlags p_usageFlags, bool p_wide) =>
		p_name is nameof(TagQuery._QueryType) or nameof(TagQuery._Tags);
}
#endif
