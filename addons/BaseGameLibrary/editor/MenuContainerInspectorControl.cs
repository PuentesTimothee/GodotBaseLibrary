// Copyright © Gamesmiths Guild.

#if TOOLS

using ElementGodot.BaseGameLibrary.UI.Core;
using Godot;

namespace ElementGodot.BaseGameLibrary.editor;

[Tool]
public partial class MenuContainerInspectorControl(CW_MenuContainer? p_menuContainer) : RichTextLabel
{
	protected CW_MenuContainer? _MenuContainer = p_menuContainer;

	public MenuContainerInspectorControl() : this(null)
	{
	}

	public override void _Process(double p_delta)
	{
		FitContent = true;

		if (_MenuContainer is not null)
			Text = $"Current tag: [{_MenuContainer.GetTag()}]";
	}
	
}
#endif
