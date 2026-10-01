// Copyright © Gamesmiths Guild.

#if TOOLS
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ElementGodot.Tags.editor;

[Tool]
public partial class TagEditorProperty : EditorProperty, ISerializationListener
{
	private TagTreeSelector? _selector;
	private readonly StringName? _restrictionString;

	public TagEditorProperty(StringName? p_restrictionString) => _restrictionString = p_restrictionString;

	public override void _Ready()
	{
		_selector = new TagTreeSelector(ESelectionMode.e_Single, _restrictionString);
		_selector.On_SelectionChanged += OnSelectionChanged;
		AddChild(_selector);
		SetBottomEditor(_selector);
	}

	public override void _UpdateProperty()
	{
		if (_selector is null || !IsInstanceValid(_selector))
			return;

		GodotObject obj = GetEditedObject();
		string propertyName = GetEditedProperty();

		Tag? tag = obj.Get(propertyName).As<Tag>();
		List<string> selection = new();
		if (tag is not null && tag.IsValid())
			selection.Add(tag.FullName().ToString());

		_selector.SetSelection(selection);
	}

	public override void _ExitTree()
	{
		ReleaseUiState();
		FreeAllChildren();
		base._ExitTree();
	}

	public void OnBeforeSerialize()
	{
		ReleaseUiState();
		FreeAllChildren();
	}

	public void OnAfterDeserialize()
	{
	}

	private void OnSelectionChanged(IReadOnlyCollection<string> p_selection)
	{
		string? key = p_selection.FirstOrDefault();
		Tag newTag = key is null ? Tag.Invalid() : Tag.RequestTag(key);

		EmitChanged(GetEditedProperty(), Variant.From(newTag));
	}

	private void ReleaseUiState()
	{
		if (_selector is not null && IsInstanceValid(_selector))
			_selector.On_SelectionChanged -= OnSelectionChanged;

		_selector = null;
	}

	private void FreeAllChildren()
	{
		for (int i = GetChildCount() - 1; i >= 0; i--)
		{
			Node child = GetChild(i);
			RemoveChild(child);
			child.Free();
		}
	}
}
#endif
