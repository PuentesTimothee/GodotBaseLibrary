// Copyright © Gamesmiths Guild.

#if TOOLS
using System;
using System.Collections.Generic;
using System.Reflection;
using BaseGameLibrary.editor;
using Godot;

// ReSharper disable once CheckNamespace
namespace Tags.editor;

[Tool]
public partial class TagInspectorPlugin : BaseEditorInspectorPlugin
{
	public TagInspectorPlugin() => _HandledTypeOfValue = new HashSet<Type> { typeof(Tag), typeof(TagContainer) };

	public override bool _ParseHandledProperty(
		GodotObject p_object,
		MemberInfo? p_memberInfo, Type p_memberType,
		string p_name,
		PropertyHint p_hintType, string p_hintString)
	{

		StringName? restrictionString;
		if (p_memberInfo is not null && p_memberInfo.GetCustomAttribute<TagAttribute>() is { } sCustom)
			restrictionString = sCustom._Restriction;
		else
			restrictionString = new StringName();

		if (p_memberType == typeof(Tag))
		{
			AddPropertyEditor(p_name, new TagEditorProperty(restrictionString));
			return true;
		}
		
		if (p_memberType == typeof(TagContainer))
		{
			AddPropertyEditor(p_name, new TagContainerEditorProperty(restrictionString));
			return true;
		}

		return false;
	}
}
#endif
