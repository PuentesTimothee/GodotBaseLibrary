using ElementGodot.BaseGameLibrary.Datas;
using ElementGodot.BaseGameLibrary.Helpers;
using ElementGodot.BaseGameLibrary.Tags;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Settings;

public enum ESettingsSection
{
    e_Undefined,
    e_Graphic,
    e_Game,
    e_Controls
};


public class GameSettings_Tags
{
    public static void Init()
    {
        GameSettings_Tags.Game_Locale = Tag.RequestTag("settings.game.locale", ETagFetch.e_CreateOnError);
        GameSettings_Tags.Graphical_Resolution = Tag.RequestTag("settings.graphical.resolution", ETagFetch.e_CreateOnError);
        GameSettings_Tags.Graphical_DisplayType = Tag.RequestTag("settings.graphical.displayType", ETagFetch.e_CreateOnError);
        GameSettings_Tags.Graphical_VSync = Tag.RequestTag("settings.graphical.vsync", ETagFetch.e_CreateOnError);
    }
    
    public static Tag Game_Locale = null!;
    
    public static Tag Graphical_Resolution = null!;
    public static Tag Graphical_DisplayType = null!;
    public static Tag Graphical_VSync = null!;
}


[GlobalClass]
public partial class GameSettings : Datas.Singleton<GameSettings>
{
    public Godot.Collections.Dictionary<StringName, GameSingleSetting> _Values = new ();
    
    protected const string FULL_PATH_DATA = SingletonHelper.PATH_DATA + "/BaseSettings.json";

    public TType? GetSetting<TType>(Tag p_tag) where TType : Resource => _Values[p_tag._StringTag] as TType;
    public TType? GetSettingValue<TType>(Tag p_tag)
        {
            if (_Values[p_tag._StringTag] is GameSingleSetting_Specific<TType> sVal)
                return sVal.GetValue();
            return default;
        }

    public override void _Ready()
    {
        GameSettings_Tags.Init();
        
        _InitOptions();
    }
    

    protected override bool _LoadFromJson()
    {
        foreach (var (_, section) in _Values)
            section.LoadValueFromState();
        
        MyLogger._LogTextCommon("LevelDescriptionManager: Loading Resource: Started.");
        if (TimHelpers._LoadJson(GameSettings.FULL_PATH_DATA) is { } sJsonData)
        {
            if (sJsonData.Data.VariantType == Variant.Type.Dictionary)
            {
                Dictionary sDic = sJsonData.Data.AsGodotDictionary();
                foreach (string sEntry in sDic.Keys)
                    if (GetSettingValue<GameSingleSetting>(Tag.RequestTag(sEntry)) is {} sOption)
                        sOption.LoadValueFromSave(sDic[sEntry]);
            }
            return true;
        }
        return false;
    }
    
    protected bool _SaveToJson()
    {
        MyLogger._LogTextCommon("LevelDescriptionManager: Loading Resource: Started.");
        if (TimHelpers._WriteJson(GameSettings.FULL_PATH_DATA,_Values))
            return true;
        return false;
    }
    
    public virtual void _InitOptions()
    {
        _AddSettingFromTag(GameSettings_Tags.Game_Locale, new GameSingleSettings_Locale());
        
        _AddSettingFromTag(GameSettings_Tags.Graphical_Resolution, new GameSingleSettings_Resolution());
        _AddSettingFromTag(GameSettings_Tags.Graphical_DisplayType, new GameSingleSettings_DisplayType());
        _AddSettingFromTag(GameSettings_Tags.Graphical_VSync, new GameSingleSettings_VSync());
    }
    
    protected void _AddSettingFromTag(Tag p_sTag, GameSingleSetting p_resource) => _Values.Add(p_sTag._StringTag, p_resource);
    protected void _AddSettingFromString(StringName p_name, GameSingleSetting p_resource) => _AddSettingFromTag(Tag.RequestTag(p_name, ETagFetch.e_CreateOnError), p_resource);
}


