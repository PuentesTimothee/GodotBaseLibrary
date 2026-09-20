using System.Collections.Generic;
using System.Diagnostics;
using ElementGodot.BaseGameLibrary.Datas;
using ElementGodot.BaseGameLibrary.Helpers;

using Godot;
using Godot.Collections;

using Array = Godot.Collections.Array;
using FileAccess = Godot.FileAccess;

namespace ElementGodot.BaseGameLibrary.Tags;

public partial class TagsManager : BaseGameLibrary.Datas.Singleton<TagsManager>
{
	protected const string FULL_PATH_DATA = $"{SingletonHelper.PATH_DATA}/Tags.json";

	public TagNode? _RootNode = null;
	public HashSet<string> _LoadedTags = new();

	protected override bool _LoadFromJson()
	{
		_RootNode = new TagNode(null, "");
		
		MyLogger._LogTextCommon("TagsManager: Loading JSon: Started.");
		if (TimHelpers._LoadJson(TagsManager.FULL_PATH_DATA) is { } sJsonData)
		{
			if (sJsonData.Data.VariantType == Variant.Type.Array)
			{
				Array sArray = sJsonData.Data.AsGodotArray();
				foreach (string sEntry in sArray)
				{
					Tag.RequestTag(sEntry, ETagFetch.e_CreateOnError);
					Add(sEntry);
				}
				MyLogger._LogTextCommon($"TagsManager: Loading JSon: Sucess.");
				return true;
			}
			MyLogger._LogErrorCommon($"TagsManager: Loading JSon: Invalid Root Type Found.");
		}
		return false;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		DestroyTagTree();
	}

	public TagNode? GetLinkedNode(Tag p_tag)
	{
		Debug.Assert(_RootNode is not null, nameof(TagsManager._RootNode) + $" is null. TagManager.GetLinkedNode(${p_tag._StringTag})");
		string[] sStrings = p_tag._StringTag.ToString().Split('.');
		return _RootNode!.FindLinkedNode(sStrings, 0);
	}
	
	public bool Contains(StringName p_tag)
	{
		Debug.Assert(_RootNode is not null, nameof(TagsManager._RootNode) + $" is null. TagManager.Contains(${p_tag})");
		string[] sStrings = p_tag.ToString().Split('.');
		return _RootNode!.FindLinkedNode(sStrings, 0) is not null;
	}

	public bool Add(StringName p_tag)
	{
		Debug.Assert(_RootNode is not null, nameof(TagsManager._RootNode) + $" is null. TagManager.Add(${p_tag})");
		
		if (!OS.HasFeature("editor"))
			_LoadedTags.Add(p_tag.ToString());
		string[] sStrings = p_tag.ToString().Split('.');
		_RootNode!.AddChild(sStrings, 0);     
		return true;
	}
	
	public bool _SaveCurrentTags()
	{
		if (!OS.HasFeature("editor"))
			return true;
		
		Array<StringName> sData = new();
		foreach (StringName stringName in _LoadedTags)
			sData.Add(stringName);
		
		Json sJsonData = new();
		sJsonData.Data = sData;

		if (FileAccess.Open(TagsManager.FULL_PATH_DATA, FileAccess.ModeFlags.Write) is { } file)
		{	
			if (!file.StoreString(Json.Stringify(sData)))
			{
				MyLogger._LogErrorCommon($"TagsManager: JsonWrite :Error");
				file.Close();
				return false;
			}

			file.Close();
		}
		return true;
	}

	public void DestroyTagTree()
	{
		MyLogger._LogTextCommon("TagsManager: Destroying Tag Tree: Started.");
		_RootNode?.DestroyNodes();
		_RootNode?.Free();
		MyLogger._LogTextCommon("TagsManager: Destroying Tag Tree: Finished.");
	}
}
