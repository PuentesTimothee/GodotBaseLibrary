using System;
using System.Collections.Generic;
using System.Linq;

using Godot;
using Tag = ElementGodot.Tags.Tag;

namespace ElementGodot.BaseGameLibrary.Helpers;

/// <summary>
/// </summary>
[Serializable]
public class MissingNodeException : Exception
{
}


public static class TimHelpers
{
	public static TType? FindParentOfType<TType>(this Node p_node) where TType : Node
	{
		Node? pParent = p_node.GetParent();
		while (pParent is not null)
		{
			if (pParent is TType tryCast)
				return tryCast;
			pParent = pParent.GetParent();
		}

		return null;
	}

	public static string Tr_Format(this GodotObject p_object, string p_string, params object?[] p_args) => 
		string.Format(p_object.Tr(p_string), p_args);
	
	public static List<TEnum> _GetEnumList<TEnum>() where TEnum : Enum  =>
		((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();

	
	//Tag
	public static bool Equals(this Tag p_tabA, object? p_obj) =>
		ReferenceEquals(p_tabA, p_obj) || (p_obj is Tag other && p_tabA.Equals(other));

	public static int _GetHashCode(this Tag p_tab) =>
		p_tab.GetHashCode();
	//Tag

	public static bool _CheckAndAssertNull(object p_obj)
	{
		if (p_obj is null)
			throw new MissingNodeException();
		return true;
	}

	public static string _TextWithColor(string p_sText, Color p_sColor) =>
		$"[color={p_sColor.ToHtml(false)}]{p_sText}[/color]";

	public static Json? _LoadJson(string p_jsonFilePath)
	{
		if (FileAccess.Open(p_jsonFilePath, FileAccess.ModeFlags.Read) is { } file)
		{
			Json sJsonData = new();
			Error error = sJsonData.Parse(file.GetAsText());
			if (error is not Error.Ok)
			{
				MyLogger._LogErrorCommon($"JsonParse:Error:{error}");
				file.Close();
				return null;
			}

			file.Close();
			return sJsonData;
		}

		return null;
	}
	public static bool _WriteJson(string p_jsonFilePath, Variant p_sData)
	{
		if (FileAccess.Open(p_jsonFilePath, FileAccess.ModeFlags.Write) is { } file)
		{
			if (!file.StoreString(Json.Stringify(p_sData, " ")))
			{
				MyLogger._LogErrorCommon($"TimHelpers: Write JSon [${p_jsonFilePath}]: Writing Failed");
				file.Close();
				return false;
			}

			file.Close();
			MyLogger._LogTextCommon($"TimHelpers: Write JSon [${p_jsonFilePath}]: Sucess");
			return true;
		}

		MyLogger._LogErrorCommon($"TimHelpers: Write JSon [${p_jsonFilePath}]: Opening Failed");
		return false;
	}
}
