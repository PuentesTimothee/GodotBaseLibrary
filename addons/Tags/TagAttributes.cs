using System;
using Godot;

namespace ElementGodot.Tags;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TagAttribute(string p_restrictionTag) : Attribute
{
    public StringName _Restriction = p_restrictionTag;
}
