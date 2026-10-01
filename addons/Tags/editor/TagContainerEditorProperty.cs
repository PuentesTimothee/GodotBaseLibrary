// Copyright © Gamesmiths Guild.

#if TOOLS
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace ElementGodot.Tags.editor;

[Tool]
public partial class TagContainerEditorProperty : EditorProperty, ISerializationListener
{
	private TagContainerInspectorControl? _editor;
	private readonly StringName? _restrictionString;

	public TagContainerEditorProperty(StringName? p_restrictionString) => _restrictionString = p_restrictionString;

	public override void _Ready()
	{
		_editor = new TagContainerInspectorControl(_restrictionString);
		_editor.On_ValueChanged += OnValueChanged;
		AddChild(_editor);
		SetBottomEditor(_editor);
	}

	public override void _UpdateProperty()
	{
		if (_editor is null || !TagContainerEditorProperty.IsInstanceValid(_editor))
			return;

		GodotObject obj = GetEditedObject();
		string propertyName = GetEditedProperty();

		TagContainer? container = obj.Get(propertyName).As<TagContainer>();
		_editor.SetValue(container ?? new TagContainer());
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

	private void OnValueChanged(Array<string> p_value)
	{
		HashSet<Tag> tags = new();
		foreach (string key in p_value)
			tags.Add(Tag.RequestTag(key));

		EmitChanged(GetEditedProperty(), Variant.From(new TagContainer(tags)));
	}

	private void ReleaseUiState()
	{
		if (_editor is not null && TagContainerEditorProperty.IsInstanceValid(_editor))
			_editor.On_ValueChanged -= OnValueChanged;

		_editor = null;
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
