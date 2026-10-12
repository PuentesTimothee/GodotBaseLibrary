using System;
using Godot;

// ReSharper disable once CheckNamespace
namespace Tags;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TagAttribute(string p_restrictionTag) : Attribute
{
    public StringName _Restriction = p_restrictionTag;
}
