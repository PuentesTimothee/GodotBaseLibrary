// Copyright © Gamesmiths Guild.

#if TOOLS

using BaseGameLibrary.UI.Core;
using Godot;

// ReSharper disable once CheckNamespace
namespace BaseGameLibrary.editor;

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

		if (_MenuContainer is not null && IsInstanceValid(_MenuContainer))
			Text = $"Current tag: [{_MenuContainer.GetTag()}]";
	}
	
}
#endif
