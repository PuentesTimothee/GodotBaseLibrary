// Copyright © Gamesmiths Guild.

#if TOOLS
using Godot;

namespace ElementGodot.Tags.editor;

[Tool]
public partial class TagInspectorPlugin : EditorInspectorPlugin
{
	public override bool _CanHandle(GodotObject p_object) => p_object is Tag;

	public override bool _ParseProperty(
		GodotObject p_object,
		Variant.Type p_type,
		string p_name,
		PropertyHint p_hintType,
		string p_hintString,
		PropertyUsageFlags p_usageFlags,
		bool p_wide)
	{
		var prop = new TagEditorProperty();
		AddPropertyEditor(p_name, prop);
		return true;
	}
}
#endif
