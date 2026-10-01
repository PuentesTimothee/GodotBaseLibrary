// Copyright © Gamesmiths Guild.

#if TOOLS

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ElementGodot.Tags.editor;

[Tool]
public partial class TagInspectorControl : VBoxContainer
{
	private readonly StringName? _restrictionString;

	private TagTreeSelector? _selector;
	private StringName? _currentValue = null;

	public TagInspectorControl() { }
	public TagInspectorControl(StringName p_restrictionString) => _restrictionString = p_restrictionString;

	public event Action<Tag>? On_ValueChanged;

	public override void _Ready()
	{
		SizeFlagsHorizontal = SizeFlags.ExpandFill;

		_selector = new TagTreeSelector(ESelectionMode.e_Single, _restrictionString);
		_selector.SetSelection(ToSelection(_currentValue));
		_selector.On_SelectionChanged += OnSelectionChanged;
		AddChild(_selector);
	}

	public override void _ExitTree()
	{
		if (_selector is not null && IsInstanceValid(_selector))
			_selector.On_SelectionChanged -= OnSelectionChanged;

		On_ValueChanged = null;
		_selector = null;
		base._ExitTree();
	}

	public void SetValue(Tag p_value) => SetValue(p_value.FullName());

	public void SetValue(StringName p_sData)
	{
		_currentValue = p_sData;
		_selector?.SetSelection(ToSelection(_currentValue));
	}

	private static IEnumerable<string> ToSelection(StringName? p_value) =>
		p_value is null ? Enumerable.Empty<string>() : [p_value];

	private void OnSelectionChanged(IReadOnlyCollection<string> p_selection)
	{
		string? tag = p_selection.FirstOrDefault();

		_currentValue = tag is null ? null : new StringName(tag);
		On_ValueChanged?.Invoke(tag is null ? Tag.Invalid() : Tag.RequestTag(tag));
	}
}
#endif
