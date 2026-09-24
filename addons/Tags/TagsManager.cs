using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ElementGodot.BaseGameLibrary;
using ElementGodot.BaseGameLibrary.Datas;
using ElementGodot.BaseGameLibrary.Helpers;

using Godot;
using Array = Godot.Collections.Array;

namespace ElementGodot.Tags;

[Tool]
public partial class TagsManager : Singleton<TagsManager>
{
	protected const string FULL_PATH_DATA = $"{SingletonHelper.PATH_DATA}/Tags.json";

	public TagNode? _RootNode = null;
	public System.Collections.Generic.HashSet<string> _LoadedTags = new();

	public bool _SaveOnTick = false;

	protected override bool _LoadFromJson()
	{
		_RootNode = new TagNode(null, "");
		
		MyLogger._LogTextCommon("TagsManager: Loading JSon: Started.");
		if (TimHelpers._LoadJson(TagsManager.FULL_PATH_DATA) is { } sJsonData)
		{
			_RootNode.AddChild(sJsonData.Data);
			if (OS.GetCmdlineUserArgs().Contains("--exportTag"))
				_SaveCurrentTags();
			MyLogger._LogTextCommon($"TagsManager: Loading JSon: Sucess.");
			return true;
		}
		return false;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		DestroyTagTree();
	}

	public override void _Process(double p_delta)
	{
		base._Process(p_delta);
		
		if (_SaveOnTick)
			_SaveCurrentTags();
		_SaveOnTick = false;
	}

	private TagNode? Find(StringName p_tag)
	{
		Debug.Assert(_RootNode is not null, nameof(TagsManager._RootNode) + $" is null. TagManager.Contains(${p_tag})");
		string[] sStrings = p_tag.ToString().Split('.');
		return _RootNode!.FindLinkedNode(sStrings, 0);
	}
	
	public bool Contains(StringName p_tag) => Find(p_tag) != null;

	public bool Add(StringName p_tag)
	{
		Debug.Assert(_RootNode is not null, nameof(TagsManager._RootNode) + $" is null. TagManager.Add(${p_tag})");

		_RootNode!.AddChild(p_tag.ToString().Split('.'));     
		
		if (!OS.HasFeature("editor"))
			_LoadedTags.Add(p_tag.ToString());
		return true;
	}
	
	public bool _SaveCurrentTags()
	{
		if (!MainSceneBase.IsTestMode)
			return true;

		if (_RootNode is null)
		{
			MyLogger._LogTextCommon("TagsManager: No root Node");
			throw new InvalidDataException("TagsManager: No root Node");
		}

		var a = new GameDatasManager();
		AddChild(a);
		RemoveChild(a);

		if (!TimHelpers._WriteJson(TagsManager.FULL_PATH_DATA,_RootNode.SerializeToVariant()))
		{	
			MyLogger._LogErrorCommon($"TagsManager: JsonWrite :Error");
			return false;
		}
		
		MyLogger._LogTextCommon("TagsManager: Successfuly saved");
		return true;
	}

	public void DestroyTagTree()
	{
		MyLogger._LogTextCommon("TagsManager: Destroying Tag Tree: Started.");
		_RootNode?.DestroyNodes();
		MyLogger._LogTextCommon("TagsManager: Destroying Tag Tree: Finished.");
	}
	
	public static TagRessource _TryToCreateMissingStateFile(Tag p_sTag)
	{
		TagRessource sRessource = TagRessource.MakeTagRessource(p_sTag.FullName());
		
		string sPath = $"{SingletonHelper.PATH_DATA}/Tag/Tag_{p_sTag}.tres";
		if (ResourceLoader.Exists(sPath))
			return sRessource;

		if (ResourceSaver.Save(sRessource, sPath) is var error and not Error.Ok)
		{
			MyLogger._LogErrorCommon($"Couldn't Save Missing Tag with ID: {p_sTag} [{error}]");
			return sRessource;
		}

		MyLogger._LogTextCommon($"Created Missing Tag with ID: {p_sTag}");
		return sRessource;
	}
	
	public static Tag RequestTag(StringName p_tag, ETagFetch p_errorGestion = ETagFetch.e_Default) =>
		Instance._RequestTag(p_tag, p_errorGestion) ?? throw new AccessViolationException($"Tried to request a tag too soon");

	private Tag _RequestTag(StringName p_tag, ETagFetch p_errorGestion = ETagFetch.e_Default)
	{
		StringName sTagName = p_tag.ToString().ToLower();

		TagNode? tagNode = Find(sTagName);

		if (tagNode is null)
		{
			switch (p_errorGestion)
			{
				case ETagFetch.e_CreateOnError:
					Add(sTagName);
					_SaveOnTick = true;
					tagNode = Find(sTagName);
					break;
				case ETagFetch.e_ThrowOnError:
					throw new TagNotRegisteredException(sTagName);
			}
		}

		if (tagNode is not null)
			return new Tag(tagNode);
		return Tag.Invalid();
	}
}
