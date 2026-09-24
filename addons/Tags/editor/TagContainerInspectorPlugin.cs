// Copyright © Gamesmiths Guild.

#if TOOLS
using Godot;

namespace ElementGodot.Tags.editor;

public partial class TagContainerInspectorPlugin : EditorInspectorPlugin
{
	public override bool _CanHandle(GodotObject p_object) => p_object is TagContainerResource;

	public override bool _ParseProperty(
		GodotObject p_object,
		Variant.Type p_type,
		string p_name,
		PropertyHint p_hintType,
		string p_hintString,
		PropertyUsageFlags p_usageFlags,
		bool p_wide)
	{
		if (p_name != nameof(TagContainer._Tags))
			return false;

		var prop = new TagContainerEditorProperty();
		AddPropertyEditor(p_name, prop);
		return true;
	}
}
#endif
