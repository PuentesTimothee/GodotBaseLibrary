// Copyright © Gamesmiths Guild.

#if TOOLS
using System;
using ElementGodot.BaseGameLibrary.UI.Core;
using Godot;

namespace ElementGodot.BaseGameLibrary.editor;

public partial class MenuContainerInspectorPlugin : BaseEditorInspectorPlugin
{
	public override bool _CanHandle(GodotObject p_object) => p_object is CW_MenuContainer;

	public override void _ParseCategory(GodotObject p_godotObject, string p_category)
	{
		base._ParseCategory(p_godotObject, p_category);
		if (p_category == nameof(CW_MenuContainer))
			AddCustomControl(new MenuContainerInspectorControl(p_godotObject as CW_MenuContainer ?? throw new InvalidOperationException()));
	}
}
#endif
