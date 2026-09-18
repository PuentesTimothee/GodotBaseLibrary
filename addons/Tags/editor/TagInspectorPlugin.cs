// Copyright © Gamesmiths Guild.

#if TOOLS
using Godot;

namespace ElementGodot.BaseGameLibrary.Tags.editor;

[Tool]
public partial class TagInspectorPlugin : EditorInspectorPlugin
{
	public override bool _CanHandle(GodotObject p_object)
	{
		if (p_object is Tag a)
			return true;

		if (p_object is Tag)
			return true;
		return p_object is Tag;
	}

	public override bool _ParseProperty(
		GodotObject p_object,
		Variant.Type p_type,
		string p_name,
		PropertyHint p_hintType,
		string p_hintString,
		PropertyUsageFlags p_usageFlags,
		bool p_wide)
	{
		if (p_name != nameof(Tag._StringTag))
			return false;
		
		var prop = new TagEditorProperty();
		AddPropertyEditor(p_name, prop);
		return true;
	}
}
#endif
