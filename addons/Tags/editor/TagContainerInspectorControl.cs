// Copyright © Gamesmiths Guild.

#if TOOLS

using System;
using System.Collections.Generic;
using Godot;
using GodotStringArray = Godot.Collections.Array<string>;

namespace ElementGodot.Tags.editor;

[Tool]
public partial class TagContainerInspectorControl : VBoxContainer
{
	private readonly StringName? _restrictionString;

	private TagTreeSelector? _selector;
	private Label? _selectedLabel;
	private GodotStringArray _currentValue = [];

	public event Action<GodotStringArray>? On_ValueChanged;

	public TagContainerInspectorControl() { }

	public TagContainerInspectorControl(StringName? p_restrictionString) => _restrictionString = p_restrictionString;

	public override void _Ready()
	{
		SizeFlagsHorizontal = SizeFlags.ExpandFill;

		_selector = new TagTreeSelector(ESelectionMode.e_Multiple, _restrictionString);
		_selector.SetSelection(_currentValue);
		_selector.On_SelectionChanged += OnSelectionChanged;
		AddChild(_selector);

		_selectedLabel = new Label
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		AddChild(_selectedLabel);
		RefreshSelectedLabel();
	}

	public override void _ExitTree()
	{
		if (_selector is not null && IsInstanceValid(_selector))
			_selector.On_SelectionChanged -= OnSelectionChanged;

		On_ValueChanged = null;
		_selector = null;
		_selectedLabel = null;
		base._ExitTree();
	}

	public void SetValue(TagContainer p_value)
	{
		GodotStringArray sArray = new();
		foreach (Tag sTag in p_value._Tags)
			sArray.Add(sTag.FullName());
		SetValue(sArray);
	}

	public void SetValue(GodotStringArray p_sArray)
	{
		_currentValue = [];
		_currentValue.AddRange(p_sArray);

		_selector?.SetSelection(_currentValue);
		RefreshSelectedLabel();
	}

	private void OnSelectionChanged(IReadOnlyCollection<string> p_selection)
	{
		GodotStringArray newValue = new();
		newValue.AddRange(p_selection);

		_currentValue = newValue;
		RefreshSelectedLabel();
		On_ValueChanged?.Invoke(newValue);
	}

	private void RefreshSelectedLabel()
	{
		if (_selectedLabel is null)
			return;

		List<string> lines = new();
		foreach (string key in _currentValue)
			lines.Add($"- {key}");

		_selectedLabel.Text = lines.Count == 0 ? "No tag selected" : string.Join("\n", lines);
	}
}
#endif
