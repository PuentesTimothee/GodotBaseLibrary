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
	private readonly Dictionary<TreeItem, TagNode> _treeItemToNode = [];

	private TagsManager? _tagsManager;

	private Tree? _tree;
	private LineEdit? _tagNameTextField;
	private Button? _addTagButton;
	private Button? _refeshTagButton;

	private Texture2D? _addIcon;
	private Texture2D? _removeIcon;

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

		_addIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Add", "EditorIcons");
		_removeIcon = EditorInterface.Singleton.GetEditorTheme().GetIcon("Remove", "EditorIcons");

		BuildUi();
		ConstructTagTree();

		_tree!.ButtonClicked += TreeButtonClicked;
		_addTagButton!.Pressed += AddTagButton_Pressed;
		_refeshTagButton!.Pressed += RefreshTreeButton_Pressed;
	}

	public void OnBeforeSerialize()
	{
		if (_tree is not null)
			_tree.ButtonClicked -= TreeButtonClicked;

		if (_addTagButton is not null)
			_addTagButton.Pressed -= AddTagButton_Pressed;
	}

	public void OnAfterDeserialize()
	{
		if (_tree is not null)
			_tree.ButtonClicked += TreeButtonClicked;

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

		_tree = new Tree
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill,
		};

		vBox.AddChild(_tree);
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
		
		_tree.Clear();
		ConstructTagTree();
	}

	private void ConstructTagTree()
	{
		EnsureInitialized();

		TreeItem rootTreeNode = _tree.CreateItem();
		_tree.HideRoot = true;

		if (_tagsManager is null)
		{
			_tagsManager = new ElementGodot.Tags.TagsManager();
			GetTree().Root.AddChild(_tagsManager);
		}

		if (_tagsManager._RootNode!._Childs.Count == 0)
		{
			TreeItem childTreeNode = _tree.CreateItem(rootTreeNode);
			childTreeNode.SetText(0, "No tag has been registered yet.");
			childTreeNode.SetCustomColor(0, Color.FromHtml("EED202"));
			return;
		}

		BuildTreeRecursively(_tree, rootTreeNode, _tagsManager._RootNode);
	}

	private void BuildTreeRecursively(Tree p_tree, TreeItem p_currentTreeItem, TagNode p_currentNode)
	{
		foreach (TagNode childTagNode in p_currentNode._Childs)
		{
			TreeItem childTreeNode = p_tree.CreateItem(p_currentTreeItem);
			childTreeNode.SetText(0, childTagNode._TagKey);
			childTreeNode.AddButton(0, _addIcon);
			childTreeNode.AddButton(0, _removeIcon);

			_treeItemToNode.Add(childTreeNode, childTagNode);

			BuildTreeRecursively(p_tree, childTreeNode, childTagNode);
		}
	}

	private void TreeButtonClicked(TreeItem p_item, long p_column, long p_id, long p_mouseButtonIndex)
	{
		EnsureInitialized();

		if (p_mouseButtonIndex == 1)
		{
			if (p_id == 0)
			{
				_tagNameTextField.Text = $"{_treeItemToNode[p_item]._TagKey}.";
				_tagNameTextField.GrabFocus();
				_tagNameTextField.CaretColumn = _tagNameTextField.Text.Length;
			}

			if (p_id == 1)
			{
				TagNode selectedTag = _treeItemToNode[p_item];
				foreach (string tag in _tagsManager._LoadedTags)
				{
					if (string.Equals(tag, selectedTag._TagKey, StringComparison.OrdinalIgnoreCase) ||
						tag.StartsWith(selectedTag._TagKey + ".", StringComparison.InvariantCultureIgnoreCase))
						_tagsManager._LoadedTags.Remove(tag);
				}

				if (selectedTag._ParentTagNode is not null
					&& !_tagsManager.Contains(selectedTag._ParentTagNode._TagKey))
					_tagsManager.Add(selectedTag._ParentTagNode._TagKey);

				_tagsManager._SaveCurrentTags();
				ReconstructTreeNode();
			}
		}
	}

	[MemberNotNull(nameof(TagsEditorDock._tagsManager), nameof(TagsEditorDock._tree), nameof(TagsEditorDock._tagNameTextField))]
	private void EnsureInitialized()
	{
		Debug.Assert(
			_tree is not null, $"{nameof(TagsEditorDock._tree)} should have been initialized on _Ready().");
		Debug.Assert(
			_tagNameTextField is not null, $"{nameof(TagsEditorDock._tagNameTextField)} should have been initialized on _Ready().");
		Debug.Assert(
			_tagsManager is not null, $"{nameof(TagsEditorDock._tagsManager)} should have been initialized on _Ready().");
	}
}
#endif
