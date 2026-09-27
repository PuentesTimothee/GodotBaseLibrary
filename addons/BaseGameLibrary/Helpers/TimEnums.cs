using System;
using Godot;

namespace ElementGodot.BaseGameLibrary.Helpers;

public static class TimEnums
{
    private const string _enumPrefix = "e_";
    
    /// <summary>
    /// Strip the given prefix from the string 
    /// </summary>
    /// <param name="p_text">String to cut</param>
    /// <param name="p_prefix">Prefix to remove from "p_text"</param>
    /// <returns>A new string without the prefix if presetn else the base text</returns>
    public static string _StripPrefix(string p_text, string p_prefix) => p_text.StartsWith(p_prefix) ? p_text.Substring(p_prefix.Length) : p_text;
    /// <summary>
    /// Strip the "e_" prefix (Common in Enum) from the string 
    /// </summary>
    /// <param name="p_text">String to cut</param>
    /// <returns>A new string without the prefix if present else the base text</returns>
    public static string _StripPrefix(string p_text) => TimEnums._StripPrefix(p_text, TimEnums._enumPrefix);
    
    /// <summary>
    /// Add the given prefix to the text if not present
    /// </summary>
    /// <param name="p_text">Text to append</param>
    /// <param name="p_prefix"></param>
    /// <returns></returns>
    public static string _AddMissingPrefix(string p_text, string p_prefix) => p_text.StartsWith(p_prefix) ? p_text : p_prefix + p_text;
    public static string _AddMissingPrefix(string p_text) => TimEnums._AddMissingPrefix(p_text, TimEnums._enumPrefix);
    
    public static bool TryParseWithPrefix<TEnumType>(string p_text, out TEnumType p_type) where TEnumType : struct
        => Enum.TryParse<TEnumType>(TimEnums._AddMissingPrefix(p_text), out p_type);

    public static TEnumType ParseWithPrefix<TEnumType>(string p_text) where TEnumType : struct
        => Enum.Parse<TEnumType>(TimEnums._AddMissingPrefix(p_text));
    
    public static string Tr_Enum<TEnumType>(this GodotObject p_obj, TEnumType p_type) where TEnumType : struct
        => p_obj.Tr( TimEnums._StripPrefix(typeof(TEnumType).Name, "E") + "." + TimEnums._StripPrefix(p_type.ToString()!));

}