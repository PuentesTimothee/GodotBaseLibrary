// Copyright © Gamesmiths Guild.

#if TOOLS
using System;
using System.Collections.Generic;
using System.Reflection;
using ElementGodot.BaseGameLibrary.editor;
using Godot;

namespace ElementGodot.Tags.editor;

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

		Type type = p_object.GetType();
		PropertyInfo? sProperty = type.GetProperty(p_name);
		FieldInfo? sField = type.GetField(p_name);
		StringName? restrictionString = null;

		if (p_memberType == typeof(Tag))
		{
			if (p_memberInfo is not null && p_memberInfo.GetCustomAttribute<TagAttribute>() is { } sCustom)
				restrictionString = sCustom._Restriction;
			else
				restrictionString = new StringName();

			AddPropertyEditor(p_name, new TagEditorProperty(restrictionString));
			return true;
		}
		
		if (p_memberType == typeof(TagContainer))
		{
			if (p_memberInfo is not null && p_memberInfo.GetCustomAttribute<TagAttribute>() is { } sCustom)
				restrictionString = sCustom._Restriction;
			else
				restrictionString = new StringName();

			AddPropertyEditor(p_name, new TagContainerEditorProperty(restrictionString));
			return true;
		}

		return false;
	}
}
#endif
