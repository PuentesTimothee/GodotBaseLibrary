using System;
using System.IO;
using ElementGodot.BaseGameLibrary.Datas;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;

namespace ElementGodot.Tags;

[GlobalClass, Tool]
public partial class TagRessource : Resource
{
	[Export] protected StringName _TagInternal;
	public StringName _Tag => _TagInternal;
	
	public Tag GetTag() => TagsManager.RequestTag(_TagInternal, ETagFetch.e_ThrowOnError);

	private TagRessource() => _TagInternal = Tag.InvalidString;
	private TagRessource(StringName p_stringName) => _TagInternal = p_stringName;

	public static TagRessource MakeTagRessource(StringName p_tag)
	{
		if (TagsManager.Instance.Contains(p_tag))
			return new TagRessource(p_tag);
		return new TagRessource();	
	}
	
	public override bool Equals(object? p_obj)
	{
		if (ReferenceEquals(this, p_obj))
			return true;
		if (p_obj is TagRessource tag)
			return Equals(tag);
		return this == p_obj;
	}
	
	protected bool Equals(TagRessource p_other) => _Tag.Equals(p_other._Tag);

	// ReSharper disable once NonReadonlyMemberInGetHashCode
	public override int GetHashCode() => _Tag.GetHashCode();

	public bool IsValid() => _Tag == Tag.InvalidString;
	public static TagRessource Invalid() => new (Tag.InvalidString);

	public string ReplacedName() => _Tag.ToString().Replace(".", "_");
	public override string ToString() => ReplacedName();
	
	public bool IsChildOf(TagRessource p_pOther)
	{
		if (!(p_pOther.IsValid() && IsValid()))
			return false;
		return GetTag().IsChildOf(p_pOther.GetTag());
	}
	
	public override void _ValidateProperty(Dictionary p_property)
	{
		if (p_property["name"].AsStringName() == PropertyName._Tag)
		{
			var usage = p_property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
			p_property["usage"] = (int)usage;
		}
	}
}
