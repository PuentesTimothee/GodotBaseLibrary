#if TOOLS

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ElementGodot.Tags.editor;

public enum ESelectionMode
{
	e_Single,
	e_Multiple,
	e_Create,
}

/// <summary>
/// Shared tag tree used by the inspectors and the tags dock.
/// Every key exchanged with the outside is a complete tag key (<see cref="TagNode.CompleteTagKey"/>).
/// </summary>
[Tool]
public partial class TagTreeSelector : VBoxContainer
{
	private const int ButtonId_Select = 0;
	private const int ButtonId_Add = 0;
	private const int ButtonId_Remove = 1;

	private readonly Dictionary<TreeItem, TagNode> _treeItemToNode = [];
	private readonly ESelectionMode _mode;
	private readonly TagNode? _restrictionTag;

	private Button? _headerButton;
	private ScrollContainer? _scroll;
	private Tree? _tree;
	private Texture2D? _checkedIcon;
	private Texture2D? _uncheckedIcon;
	private List<string> _selection = [];

	/// <summary>Text of the header button (Single/Multiple). Null uses a default text for the mode.</summary>
	public Func<IReadOnlyCollection<string>, string>? HeaderText { get; set; }

	/// <summary>Raised on click with the whole new selection (Single/Multiple).</summary>
	public event Action<IReadOnlyCollection<string>>? On_SelectionChanged;

	/// <summary>Raised when the add button of a tag is clicked, with that tag as parent (Create).</summary>
	public event Action<string>? On_AddChildRequested;

	/// <summary>Raised when the remove button of a tag is clicked (Create).</summary>
	public event Action<string>? On_RemoveRequested;

	public TagTreeSelector() => _mode = ESelectionMode.e_Single;

	public TagTreeSelector(ESelectionMode p_mode, StringName? p_restriction = null)
	{
		_mode = p_mode;

		if (p_mode != ESelectionMode.e_Create && !string.IsNullOrEmpty(p_restriction))
			_restrictionTag = Tag.RequestTag(p_restriction, ETagFetch.e_CreateOnError).GetTagNode();
	}

	public override void _Ready()
	{
		SizeFlagsHorizontal = SizeFlags.ExpandFill;

		_tree = new Tree
		{
			HideRoot = true,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill,
		};
		_tree.ButtonClicked += OnTreeButtonClicked;

		if (_mode == ESelectionMode.e_Create)
		{
			SizeFlagsVertical = SizeFlags.ExpandFill;
			AddChild(_tree);
		}
		else
		{
			_headerButton = new Button
			{
				ToggleMode = true,
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
			};
			_headerButton.Toggled += OnToggled;
			AddChild(_headerButton);

			_scroll = new ScrollContainer
			{
				Visible = false,
				CustomMinimumSize = new Vector2(0, 220),
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				SizeFlagsVertical = SizeFlags.ExpandFill,
			};
			_scroll.AddChild(_tree);
			AddChild(_scroll);
		}

		LoadIcons();
		RebuildTree();
	}

	public override void _ExitTree()
	{
		if (_headerButton is not null && IsInstanceValid(_headerButton))
			_headerButton.Toggled -= OnToggled;

		if (_tree is not null && IsInstanceValid(_tree))
			_tree.ButtonClicked -= OnTreeButtonClicked;

		On_SelectionChanged = null;
		On_AddChildRequested = null;
		On_RemoveRequested = null;
		_treeItemToNode.Clear();
		_headerButton = null;
		_scroll = null;
		_tree = null;
		_checkedIcon = null;
		_uncheckedIcon = null;
		base._ExitTree();
	}

	public void SetSelection(IEnumerable<string> p_keys)
	{
		_selection = p_keys.ToList();
		RebuildTree();
	}

	public void Refresh() => RebuildTree();

	private void LoadIcons()
	{
		Theme editorTheme = EditorInterface.Singleton.GetEditorTheme();

		(string checkedName, string uncheckedName) = _mode switch
		{
			ESelectionMode.e_Single => ("GuiRadioChecked", "GuiRadioUnchecked"),
			ESelectionMode.e_Multiple => ("GuiChecked", "GuiUnchecked"),
			_ => ("Add", "Remove"),
		};

		_checkedIcon = editorTheme.GetIcon(checkedName, "EditorIcons");
		_uncheckedIcon = editorTheme.GetIcon(uncheckedName, "EditorIcons");
	}

	private void RebuildTree()
	{
		if (_tree is null || _checkedIcon is null || _uncheckedIcon is null)
			return;

		_tree.Clear();
		_treeItemToNode.Clear();

		if (_headerButton is not null)
			_headerButton.Text = GetHeaderText();

		TreeItem root = _tree.CreateItem();
		TagNode? rootNode = _restrictionTag ?? TagsManager.Instance._RootNode;

		if (rootNode is null || rootNode._Childs.Count == 0)
		{
			TreeItem emptyItem = _tree.CreateItem(root);
			emptyItem.SetText(0, "No tag has been registered yet.");
			emptyItem.SetCustomColor(0, Color.FromHtml("EED202"));
			return;
		}

		BuildTreeRecursive(root, rootNode);
	}

	private void BuildTreeRecursive(TreeItem p_parent, TagNode p_node)
	{
		if (_tree is null)
			return;

		foreach (TagNode child in p_node._Childs)
		{
			TreeItem item = _tree.CreateItem(p_parent);
			item.SetText(0, child._TagKey);

			if (_mode == ESelectionMode.e_Create)
			{
				item.AddButton(0, _checkedIcon, ButtonId_Add);
				item.AddButton(0, _uncheckedIcon, ButtonId_Remove);
			}
			else
			{
				item.AddButton(0, _selection.Contains(child.CompleteTagKey()) ? _checkedIcon : _uncheckedIcon, ButtonId_Select);
			}

			_treeItemToNode[item] = child;
			BuildTreeRecursive(item, child);
		}
	}

	private string GetHeaderText()
	{
		if (HeaderText is not null)
			return HeaderText(_selection);

		return _mode == ESelectionMode.e_Single
			? (_selection.Count == 0 ? "None" : _selection[0])
			: $"Container (size: {_selection.Count})";
	}

	private void OnTreeButtonClicked(TreeItem p_item, long p_column, long p_id, long p_mouseButtonIndex)
	{
		if (_tree is null || !IsInstanceValid(_tree))
			return;

		if (p_mouseButtonIndex != 1 || !_treeItemToNode.TryGetValue(p_item, out TagNode? node))
			return;

		string key = node.CompleteTagKey();

		switch (_mode)
		{
			case ESelectionMode.e_Create:
				if (p_id == ButtonId_Add)
					On_AddChildRequested?.Invoke(key);
				else if (p_id == ButtonId_Remove)
					On_RemoveRequested?.Invoke(key);
				break;

			case ESelectionMode.e_Single:
				SetSelection(_selection.Contains(key) ? [] : [key]);
				On_SelectionChanged?.Invoke(_selection.ToList());
				break;

			case ESelectionMode.e_Multiple:
				List<string> newSelection = _selection.ToList();
				if (!newSelection.Remove(key))
					newSelection.Add(key);

				SetSelection(newSelection);
				On_SelectionChanged?.Invoke(newSelection);
				break;
		}
	}

	private void OnToggled(bool p_toggled)
	{
		if (_scroll is null || !IsInstanceValid(_scroll))
			return;

		_scroll.Visible = p_toggled;
	}
}
#endif
