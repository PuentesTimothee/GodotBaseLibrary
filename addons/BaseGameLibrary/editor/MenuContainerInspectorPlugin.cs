// Copyright © Gamesmiths Guild.

#if TOOLS
using System;
using ElementGodot.BaseGameLibrary.UI.Core;
using ElementGodot.Tags;
using ElementGodot.Tags.editor;
using Godot;

namespace ElementGodot.BaseGameLibrary.editor;

public partial class MenuContainerInspectorPlugin : EditorInspectorPlugin
{
	public override bool _CanHandle(GodotObject p_object) => p_object is CW_MenuContainer;

	public override bool _ParseProperty(
		GodotObject p_object,
		Variant.Type p_type,
		string p_name,
		PropertyHint p_hintType,
		string p_hintString,
		PropertyUsageFlags p_usageFlags,
		bool p_wide)
	{
		if (p_name != nameof(CW_MenuContainer._Tag))
			return false;

		AddPropertyEditor(p_name, new MenuContainerInspectorControl(p_object as CW_MenuContainer ?? throw new InvalidOperationException()));
		return true;
	}
}
#endif
