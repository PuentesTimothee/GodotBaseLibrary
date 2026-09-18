// Copyright © Gamesmiths Guild.

#if TOOLS

using System;
using System.Collections.Generic;
using Godot;
using GodotStringArray = Godot.Collections.Array<string>;

namespace ElementGodot.BaseGameLibrary.Tags.editor;

[Tool]
public partial class TagContainerSelectionControl : VBoxContainer
{
	private readonly Dictionary<TreeItem, TagNode> _treeItemToNode = [];

	private Button? _containerButton;
	private ScrollContainer? _scroll;
	private Tree? _tree;
	private Texture2D? _checkedIcon;
	private Texture2D? _uncheckedIcon;
	private GodotStringArray _currentValue = [];

	public event Action<GodotStringArray>? On_ValueChanged;

	public override void _Ready()
	{
		SizeFlagsHorizontal = SizeFlags.ExpandFill;

		_containerButton = new Button
		{
			ToggleMode = true,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		_containerButton.Toggled += OnToggled;
		AddChild(_containerButton);

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
		_tree.ButtonClicked += OnTreeButtonClicked;

		_scroll.AddChild(_tree);
		AddChild(_scroll);

		_checkedIcon = EditorInterface.Singleton
			.GetEditorTheme()
			.GetIcon("GuiChecked", "EditorIcons");
		_uncheckedIcon = EditorInterface.Singleton
			.GetEditorTheme()
			.GetIcon("GuiUnchecked", "EditorIcons");

		RebuildTree();
	}

	public override void _ExitTree()
	{
		if (_containerButton is not null && TagContainerSelectionControl.IsInstanceValid(_containerButton))
			_containerButton.Toggled -= OnToggled;

		if (_tree is not null && TagContainerSelectionControl.IsInstanceValid(_tree))
			_tree.ButtonClicked -= OnTreeButtonClicked;

		On_ValueChanged = null;
		_treeItemToNode.Clear();
		_containerButton = null;
		_scroll = null;
		_tree = null;
		_checkedIcon = null;
		_uncheckedIcon = null;
		base._ExitTree();
	}

	public void SetValue(TagContainer p_value)
	{
		GodotStringArray sArray = new();
		foreach (Tag sTag in p_value._Tags)
			sArray.Add(sTag._StringTag);
		SetValue(sArray);
	}
	
	public void SetValue(GodotStringArray sArray)
	{
		_currentValue = [];
		_currentValue.AddRange(sArray);

		if (_tree is not null)
			RebuildTree();
	}

	private void RebuildTree()
	{
		if (_tree is null || _containerButton is null || _checkedIcon is null || _uncheckedIcon is null)
			return;

		_tree.Clear();
		_treeItemToNode.Clear();
		_containerButton.Text = $"Container (size: {_currentValue.Count})";

		TreeItem root = _tree.CreateItem();
		BuildTreeRecursive(root, TagsManager.Instance._RootNode!);
	}

	private void BuildTreeRecursive(TreeItem p_parent, TagNode p_node)
	{
		if (_tree is null)
			return;

		foreach (TagNode child in p_node._Childs)
		{
			TreeItem item = _tree.CreateItem(p_parent);
			item.SetText(0, child._TagKey);
			item.AddButton(0, _currentValue.Contains(child._CompleteTagKey) ? _checkedIcon : _uncheckedIcon);
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
		if (_tree is null || !TagContainerSelectionControl.IsInstanceValid(_tree))
			return;

		if (p_mouseButtonIndex != 1 || p_id != 0)
			return;

		string tag = _treeItemToNode[p_item]._CompleteTagKey;
		var newValue = new GodotStringArray();
		newValue.AddRange(_currentValue);

		if (!newValue.Remove(tag))
			newValue.Add(tag);

		SetValue(newValue);
		On_ValueChanged?.Invoke(newValue);
	}

	private void OnToggled(bool p_toggled)
	{
		if (_scroll is null || !TagContainerSelectionControl.IsInstanceValid(_scroll))
			return;

		_scroll.Visible = p_toggled;
	}
}
#endif
