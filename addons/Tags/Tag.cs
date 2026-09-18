using System;
using System.IO;
using ElementGodot.BaseGameLibrary.Datas;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;

namespace ElementGodot.BaseGameLibrary.Tags;

public enum ETagFetch
{
	e_Default,
	e_ThrowOnError,
	e_CreateOnError
}

[GlobalClass, Tool]
public partial class Tag : Resource
{
	public delegate Tag OnTagSelected();
	
	private static readonly StringName _invalidString = "!!Invalid!!";
	
	[Export] public StringName _StringTag;
	protected TagNode? _FetchedNode;

	public Tag(StringName p_tag)
	{
		_StringTag = p_tag;
		if (p_tag != Tag._invalidString)
			_FetchedNode = TagsManager.Instance.GetLinkedNode(this);
	}
	
	public Tag() => _StringTag = Tag._invalidString;

	public override bool Equals(object? p_obj)
	{
		if (ReferenceEquals(this, p_obj))
			return true;
		if (p_obj is Tag tag)
			return Equals(tag);
		return this == p_obj;
	}

	protected bool Equals(Tag p_other) => _StringTag.Equals(p_other._StringTag);

	// ReSharper disable once NonReadonlyMemberInGetHashCode
	public override int GetHashCode() => _StringTag.GetHashCode();

	public bool IsValid() => _StringTag == Tag._invalidString;
	public static Tag Invalid() => new (Tag._invalidString);

	public string ReplacedName() => _StringTag.ToString().Replace(".", "_");
	public override string ToString() => ReplacedName();
	
	public bool IsChildOf(Tag p_pOther)
	{
		if (!(p_pOther.IsValid() && IsValid()) ||
		    _FetchedNode is null || p_pOther._FetchedNode is null)
			return false;
		return _FetchedNode.IsChildOf(_FetchedNode);
	}
	
	public static Tag RequestTag(StringName p_tag, ETagFetch p_errorGestion = ETagFetch.e_Default)
	{
		StringName sTagName = p_tag.ToString().ToLower();
		
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		if (TagsManager.Instance is not null)
			if (TagsManager.Instance.Contains(sTagName))
				return new Tag(sTagName);

		switch (p_errorGestion)
		{
			case ETagFetch.e_ThrowOnError:
				throw new InvalidDataException($"Missing tag {sTagName}");
			case ETagFetch.e_CreateOnError:
				if (TagsManager.Instance is not null)
				{
					TagsManager.Instance.Add(sTagName);
					TagsManager.Instance._SaveCurrentTags();
					return Tag._TryToCreateMissingStateFile(new Tag(sTagName));
				}
				throw new AccessViolationException($"Tried to request a tag too soon");
		}

		return Tag.Invalid();
	}

	public static Tag _TryToCreateMissingStateFile(Tag p_sDesc)
	{
		string sPath = $"{SingletonHelper.PATH_DATA}/Tag/Tag_{p_sDesc}.tres";
		if (ResourceLoader.Exists(sPath))
			return p_sDesc;

		if (ResourceSaver.Save(p_sDesc, sPath) is var error and not Error.Ok)
		{
			MyLogger._LogErrorCommon($"Couldn't Save Missing Tag with ID: {p_sDesc} [{error}]");
			return p_sDesc;
		}

		MyLogger._LogTextCommon($"Created Missing Tag with ID: {p_sDesc}");
		return p_sDesc;
	}
}
