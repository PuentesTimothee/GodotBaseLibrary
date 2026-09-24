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
public partial class Tag : RefCounted
{
	public static readonly StringName InvalidString = "!!Invalid!!";

	protected TagNode? _FetchedNode;
	
	public Tag(TagNode p_tag) => _FetchedNode = p_tag;
	public Tag() => _FetchedNode = null;

	public bool IsValid() => _FetchedNode is not null;
	public static Tag Invalid() => new ();
	
	public bool IsChildOf(Tag p_pOther)
	{
		if (_FetchedNode is null || p_pOther._FetchedNode is null)
			return false;
		return _FetchedNode.IsChildOf(p_pOther._FetchedNode);
	}

	public StringName FullName() => _FetchedNode is not null ? _FetchedNode.CompleteTagKey() : InvalidString;

	public static Tag RequestTag(StringName p_tag, ETagFetch p_errorGestion = ETagFetch.e_Default) => TagsManager.RequestTag(p_tag, p_errorGestion);

}
