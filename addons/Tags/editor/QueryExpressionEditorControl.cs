// Copyright © Gamesmiths Guild.

#if TOOLS
using System;
using Godot;
using Godot.Collections;

namespace ElementGodot.Tags.editor;

[Tool]
public partial class QueryExpressionEditorControl : VBoxContainer
{
	private const float _labelWidth = 66.0f;

	private TagQuery? _query;
	private Action? _onChanged;
	private OptionButton? _expressionTypeDropdown;
	private VBoxContainer? _contentContainer;

	public event Action? On_LayoutChanged;

	public void Setup(TagQuery p_query, Action p_onChanged)
	{
		_query = p_query;
		_onChanged = p_onChanged;
		//QueryExpressionEditorControl.EnsureDefaultData(p_query);

		if (IsNodeReady())
		{
			EnsureUi();
			RefreshUi();
		}
	}

	public override void _Ready()
	{
		SizeFlagsHorizontal = SizeFlags.ExpandFill;
		EnsureUi();
		RefreshUi();
	}

	public override void _ExitTree()
	{
		if (_expressionTypeDropdown is not null && QueryExpressionEditorControl.IsInstanceValid(_expressionTypeDropdown))
		{
			_expressionTypeDropdown.ItemSelected -= OnExpressionTypeChanged;
		}

		On_LayoutChanged = null;
		_onChanged = null;
		_expressionTypeDropdown = null;
		_contentContainer = null;
		base._ExitTree();
	}

	private static HBoxContainer CreateLabeledRow(string p_labelText, Control p_editor)
	{
		var row = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		row.AddChild(new Label
		{
			Text = p_labelText,
			CustomMinimumSize = new Vector2(QueryExpressionEditorControl._labelWidth, 0),
			HorizontalAlignment = HorizontalAlignment.Right,
		});
		row.AddChild(p_editor);
		return row;
	}

	private static HBoxContainer CreateIndentedRow(Control p_editor)
	{
		var row = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		row.AddChild(new Label
		{
			CustomMinimumSize = new Vector2(QueryExpressionEditorControl._labelWidth, 0),
		});
		row.AddChild(p_editor);
		return row;
	}

	private static void ClearContainer(Control p_container)
	{
		foreach (Node child in p_container.GetChildren())
		{
			p_container.RemoveChild(child);
			child.Free();
		}
	}

	private static TagQuery CreateDefaultQueryExpression() => new TagQuery().SetType(EQueryType.e_AnyMatchTag);

	private void EnsureUi()
	{
		if (_expressionTypeDropdown is not null && _contentContainer is not null)
		{
			return;
		}

		_expressionTypeDropdown = new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		foreach (EQueryType value in Enum.GetValues<EQueryType>())
			_expressionTypeDropdown.AddItem(value.ToString());

		_expressionTypeDropdown.ItemSelected += OnExpressionTypeChanged;
		AddChild(QueryExpressionEditorControl.CreateLabeledRow("Type:", _expressionTypeDropdown));

		_contentContainer = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		AddChild(_contentContainer);
	}

	private void RefreshUi()
	{
		if (_query is null || _expressionTypeDropdown is null || _contentContainer is null)
		{
			return;
		}

		//QueryExpressionEditorControl.EnsureDefaultData(_query);
		_expressionTypeDropdown.Selected = (int)_query._QueryType;
		RefreshContent();
	}

	private void RefreshContent()
	{
		if (_query is null || _contentContainer is null)
		{
			return;
		}

		QueryExpressionEditorControl.ClearContainer(_contentContainer);

		if (_query._QueryType == EQueryType.e_Undefined)
		{
			_contentContainer.AddChild(new Label
			{
				Text = "Select an expression type.",
			});
			RaiseLayoutChanged();
			return;
		}

		if (_query.IsExpression())
			BuildExpressionsEditor();
		else
			BuildTagContainerEditor();

		RaiseLayoutChanged();
	}

	private void BuildExpressionsEditor()
	{
		if (_query is null || _contentContainer is null)
			return;

		var addButton = new Button
		{
			Text = "Add Expression",
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
		};
		addButton.Pressed += OnAddExpressionPressed;
		_contentContainer.AddChild(QueryExpressionEditorControl.CreateLabeledRow("Exprs:", addButton));

		if (_query._Expressions.Count == 0)
		{
			_contentContainer.AddChild(QueryExpressionEditorControl.CreateIndentedRow(new Label { Text = "(none)" }));
			return;
		}

		int i = 0;
		foreach (TagQuery expression in _query._Expressions)
		{
			var itemRoot = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
			var headerRow = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
			headerRow.AddChild(new Label
			{
				Text = $"Item {i + 1}:",
				CustomMinimumSize = new Vector2(QueryExpressionEditorControl._labelWidth, 0),
				HorizontalAlignment = HorizontalAlignment.Right,
			});

			var removeButton = new Button { Text = "Remove" };
			int index = i;
			removeButton.Pressed += () => OnRemoveExpressionPressed(index);
			headerRow.AddChild(removeButton);
			itemRoot.AddChild(headerRow);

			var nestedEditor = new QueryExpressionEditorControl();
			nestedEditor.Setup(expression, NotifyChanged);
			nestedEditor.On_LayoutChanged += RaiseLayoutChanged;
			itemRoot.AddChild(QueryExpressionEditorControl.CreateIndentedRow(nestedEditor));

			_contentContainer.AddChild(itemRoot);
			i++;
		}
	}

	private void BuildTagContainerEditor()
	{
		if (_query is null || _contentContainer is null)
			return;
		
		TagContainerInspectorControl tagContainerEditor = new();
		tagContainerEditor.SetValue(_query._Tags);
		tagContainerEditor.On_ValueChanged += OnTagContainerChanged;
		_contentContainer.AddChild(QueryExpressionEditorControl.CreateLabeledRow("Tags:", tagContainerEditor));
	}

	private void OnExpressionTypeChanged(long p_index)
	{
		if (_query is null)
			return;

		_query._QueryType = (EQueryType)(int)p_index;
		RefreshUi();
		NotifyChanged();
	}

	private void OnAddExpressionPressed()
	{
		if (_query is null)
			return;

		_query._Expressions.Add(QueryExpressionEditorControl.CreateDefaultQueryExpression());
		RefreshUi();
		NotifyChanged();
	}

	private void OnRemoveExpressionPressed(int p_index)
	{
		if (_query?._Expressions is null || p_index < 0 || p_index >= _query._Expressions.Count)
			return;

		_query._Expressions.RemoveAt(p_index);
		RefreshUi();
		NotifyChanged();
	}

	private void OnTagContainerChanged(Array<string> p_tags)
	{
		if (_query is null)
		{
			return;
		}

		_query._Tags._Tags.Clear();
		foreach (StringName tag in p_tags)
			_query._Tags.Add(Tag.RequestTag(tag));
		NotifyChanged();
	}

	private void NotifyChanged()
	{
		_query?.EmitChanged();
		_onChanged?.Invoke();
		RaiseLayoutChanged();
	}

	private void RaiseLayoutChanged()
	{
		On_LayoutChanged?.Invoke();
	}
}
#endif
