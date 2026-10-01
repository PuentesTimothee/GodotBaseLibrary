// Copyright © Gamesmiths Guild.

#if TOOLS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace ElementGodot.Tags.editor;

/// <summary>
/// Editor dock for managing gameplay tags.
/// </summary>
[Tool]
public partial class TagsEditorDock : EditorDock, ISerializationListener
{
	private TagsManager? _tagsManager;

	private TagTreeSelector? _selector;
	private LineEdit? _tagNameTextField;
	private Button? _addTagButton;
	private Button? _refeshTagButton;

	public TagsEditorDock()
	{
		Title = "Tags";
		//DockIcon = GD.Load<Texture2D>("uid://cu6ncpuumjo20");
		DefaultSlot = DockSlot.RightUl;
	}

	public override void _Ready()
	{
		base._Ready();

		_tagsManager = TagsManager.Instance;
		//GetTree().Root.AddChild(_tagsManager);

		BuildUi();

		_selector!.On_AddChildRequested += OnAddChildRequested;
		_selector!.On_RemoveRequested += OnRemoveRequested;
		_addTagButton!.Pressed += AddTagButton_Pressed;
		_refeshTagButton!.Pressed += RefreshTreeButton_Pressed;
	}

	public void OnBeforeSerialize()
	{
		if (_selector is not null)
		{
			_selector.On_AddChildRequested -= OnAddChildRequested;
			_selector.On_RemoveRequested -= OnRemoveRequested;
		}

		if (_addTagButton is not null)
			_addTagButton.Pressed -= AddTagButton_Pressed;
	}

	public void OnAfterDeserialize()
	{
		if (_selector is not null)
		{
			_selector.On_AddChildRequested += OnAddChildRequested;
			_selector.On_RemoveRequested += OnRemoveRequested;
		}

		if (_addTagButton is not null)
			_addTagButton.Pressed += AddTagButton_Pressed;

		_tagsManager = ElementGodot.Tags.TagsManager.Instance;
		ReconstructTreeNode();
	}

	private void BuildUi()
	{
		var vBox = new VBoxContainer
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill,
		};

		AddChild(vBox);

		var hBox = new HBoxContainer
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};

		vBox.AddChild(hBox);

		var label = new Label
		{
			Text = "Tag Name:",
		};

		hBox.AddChild(label);

		_tagNameTextField = new LineEdit
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};

		hBox.AddChild(_tagNameTextField);

		_addTagButton = new Button
		{
			Text = "Add Tag",
		};

		hBox.AddChild(_addTagButton);
		
		_refeshTagButton = new Button
		{
			Text = "Refresh",
		};

		hBox.AddChild(_refeshTagButton);

		_selector = new TagTreeSelector(ESelectionMode.e_Create);

		vBox.AddChild(_selector);
	}


	private void RefreshTreeButton_Pressed()
	{
	}
	
	private void AddTagButton_Pressed()
	{
		EnsureInitialized();

		if (_tagsManager.Contains(_tagNameTextField.Text))
		{
			GD.PushWarning($"Tag [{_tagNameTextField.Text}] is already present in the manager.");
			return;
		}

		Tag.RequestTag(_tagNameTextField.Text, ETagFetch.e_CreateOnError);
		ReconstructTreeNode();
	}

	private void ReconstructTreeNode()
	{
		EnsureInitialized();

		_selector.Refresh();
	}

	private void OnAddChildRequested(string p_tagKey)
	{
		EnsureInitialized();

		_tagNameTextField.Text = $"{p_tagKey}.";
		_tagNameTextField.GrabFocus();
		_tagNameTextField.CaretColumn = _tagNameTextField.Text.Length;
	}

	private void OnRemoveRequested(string p_tagKey)
	{
		EnsureInitialized();

		_tagsManager._LoadedTags.RemoveWhere((string tag) =>
			string.Equals(tag, p_tagKey, StringComparison.OrdinalIgnoreCase) ||
			tag.StartsWith(p_tagKey + ".", StringComparison.InvariantCultureIgnoreCase));

		int lastSeparator = p_tagKey.LastIndexOf('.');
		if (lastSeparator > 0)
		{
			string parentKey = p_tagKey[..lastSeparator];
			if (!_tagsManager.Contains(parentKey))
				_tagsManager.Add(parentKey);
		}

		_tagsManager._SaveCurrentTags();
		ReconstructTreeNode();
	}

	[MemberNotNull(nameof(TagsEditorDock._tagsManager), nameof(TagsEditorDock._selector), nameof(TagsEditorDock._tagNameTextField))]
	private void EnsureInitialized()
	{
		Debug.Assert(
			_selector is not null, $"{nameof(TagsEditorDock._selector)} should have been initialized on _Ready().");
		Debug.Assert(
			_tagNameTextField is not null, $"{nameof(TagsEditorDock._tagNameTextField)} should have been initialized on _Ready().");
		Debug.Assert(
			_tagsManager is not null, $"{nameof(TagsEditorDock._tagsManager)} should have been initialized on _Ready().");
	}
}
#endif
