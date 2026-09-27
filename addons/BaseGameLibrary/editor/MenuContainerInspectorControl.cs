// Copyright © Gamesmiths Guild.

#if TOOLS

using ElementGodot.BaseGameLibrary.UI.Core;
using Godot;

namespace ElementGodot.BaseGameLibrary.editor;

[Tool]
public partial class MenuContainerInspectorControl(CW_MenuContainer p_menuContainer) : RichTextLabel
{
	protected CW_MenuContainer _MenuContainer = p_menuContainer;
	
	public override void _Process(double p_delta)
	{
		Text = $"Current tag {_MenuContainer._Tag}";
	}
	
}
#endif
