using System;
using System.IO;
using ElementGodot.BaseGameLibrary.Datas;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;

namespace ElementGodot.Tags;

public enum ETagFetch
{
	e_Default,
	e_ThrowOnError,
	e_CreateOnError
}

public class TagNotRegisteredException : Exception
{
	public TagNotRegisteredException(StringName p_tag) : base($"[TagNotRegisteredException] Missing tag [{p_tag}]")
	{
	}
};	

[Tool]
public partial class Tag : Resource
{
	public static readonly StringName InvalidString = "!!Invalid!!";

	private StringName _stringTagInternal = InvalidString;
	private TagNode? _fetchedNode;

	[Export]
	public StringName _StringTag
	{
		get => _stringTagInternal;
		set
		{
			_stringTagInternal = value;
			_fetchedNode = null;
		}
	}

	// The node is resolved on first access: the TagsManager may not exist yet when a saved Tag is loaded.
	public TagNode? GetTagNode()
	{
		if (_fetchedNode is null && _stringTagInternal != InvalidString && TagsManager.Instance is not null)
			_fetchedNode = TagsManager.RequestTag(_stringTagInternal).GetTagNode();
		return _fetchedNode;
	}

	public Tag(TagNode p_tag)
	{
		_fetchedNode = p_tag;
		_stringTagInternal = p_tag.CompleteTagKey();
	}

	public Tag() { }

	public bool IsValid() => GetTagNode() is not null;
	public static Tag Invalid() => new ();

	public bool IsChildOf(Tag p_pOther)
	{
		TagNode? node = GetTagNode();
		TagNode? otherNode = p_pOther.GetTagNode();
		if (node is null || otherNode is null)
			return false;
		return node.IsChildOf(otherNode);
	}

	public StringName FullName() => GetTagNode() is { } node ? node.CompleteTagKey() : InvalidString;

	public override bool Equals(object? p_obj) =>
		ReferenceEquals(this, p_obj) || (p_obj is Tag other && _stringTagInternal == other._stringTagInternal);

	// ReSharper disable once NonReadonlyMemberInGetHashCode
	public override int GetHashCode() => _stringTagInternal.GetHashCode();

	public override string ToString() => FullName();
	public static Tag RequestTag(StringName p_tag, ETagFetch p_errorGestion = ETagFetch.e_Default) => TagsManager.RequestTag(p_tag, p_errorGestion);

}
