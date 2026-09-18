// Copyright © Gamesmiths Guild.

#if TOOLS
using System.Collections.Generic;
using Godot;

namespace ElementGodot.BaseGameLibrary.Tags.editor;

[Tool]
public partial class TagEditorProperty : EditorProperty, ISerializationListener
{
	private readonly Dictionary<TreeItem, TagNode> _treeItemToNode = [];

	private VBoxContainer? _root;
	private Button? _containerButton;
	private ScrollContainer? _scroll;
	private Tree? _tree;

	private Texture2D? _checkedIcon;
	private Texture2D? _uncheckedIcon;

	private string _currentValue = string.Empty;

	public override void _Ready()
	{
		_root = new VBoxContainer
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};

		_containerButton = new Button
		{
			ToggleMode = true,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		_containerButton.Toggled += OnToggled;

		_scroll = new ScrollContainer
		{
			Visible = false,
			CustomMinimumSize = new Vector2(0, 220),
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill,
		};

		_tree = new Tree
		{
			HideRoot = true,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill,
		};

		_scroll.AddChild(_tree);

		_root.AddChild(_containerButton);
		_root.AddChild(_scroll);

		AddChild(_root);
		SetBottomEditor(_root);

		_checkedIcon = EditorInterface.Singleton
			.GetEditorTheme()
			.GetIcon("GuiRadioChecked", "EditorIcons");

		_uncheckedIcon = EditorInterface.Singleton
			.GetEditorTheme()
			.GetIcon("GuiRadioUnchecked", "EditorIcons");

		_tree.ButtonClicked += OnTreeButtonClicked;
	}

	public override void _UpdateProperty()
	{
		if (_tree is null || _containerButton is null || !TagEditorProperty.IsInstanceValid(_tree) || !TagEditorProperty.IsInstanceValid(_containerButton))
			return;

		GodotObject obj = GetEditedObject();
		string propertyName = GetEditedProperty();

		_currentValue = obj.Get(propertyName).AsString();
		RebuildTree();
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

	private void RebuildTree()
	{
		if (_tree is null || _containerButton is null || _checkedIcon is null || _uncheckedIcon is null)
		{
			return;
		}

		_tree.Clear();
		_treeItemToNode.Clear();

		_containerButton.Text = string.IsNullOrEmpty(_currentValue) ? "(none)" : _currentValue;

		TreeItem root = _tree.CreateItem();
		var tagsManager = TagsManager.Instance;

		BuildTreeRecursive(root, tagsManager._RootNode!);

		UpdateMinimumSize();
		NotifyPropertyListChanged();
	}

	private void BuildTreeRecursive(TreeItem p_parent, TagNode p_node)
	{
		if (_tree is null)
		{
			return;
		}

		foreach (TagNode child in p_node._Childs)
		{
			TreeItem item = _tree.CreateItem(p_parent);
			item.SetText(0, child._TagKey);

			bool selected = _currentValue == child._CompleteTagKey;
			item.AddButton(0, selected ? _checkedIcon : _uncheckedIcon);

			_treeItemToNode[item] = child;
			BuildTreeRecursive(item, child);
		}
	}

	private void OnTreeButtonClicked(
		TreeItem p_item,
		long p_column,
		long p_id,
		long p_mouseButtonIndex)
	{
		if (_tree is null || !TagEditorProperty.IsInstanceValid(_tree))
			return;

		if (p_mouseButtonIndex != 1 || p_id != 0)
			return;

		string newValue = _treeItemToNode[p_item]._CompleteTagKey;

		if (newValue == _currentValue)
			newValue = string.Empty;

		EmitChanged(GetEditedProperty(), newValue);
	}

	private void OnToggled(bool p_toggled)
	{
		if (_scroll is null || !TagEditorProperty.IsInstanceValid(_scroll))
			return;

		_scroll.Visible = p_toggled;

		UpdateMinimumSize();
		NotifyPropertyListChanged();
	}

	private void ReleaseUiState()
	{
		if (_containerButton is not null && TagEditorProperty.IsInstanceValid(_containerButton))
			_containerButton.Toggled -= OnToggled;

		if (_tree is not null && TagEditorProperty.IsInstanceValid(_tree))
			_tree.ButtonClicked -= OnTreeButtonClicked;

		_treeItemToNode.Clear();
		_root = null;
		_containerButton = null;
		_scroll = null;
		_tree = null;
		_checkedIcon = null;
		_uncheckedIcon = null;
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
