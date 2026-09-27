using System;
using ElementGodot.BaseGameLibrary.Helpers;
using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Datas;

public sealed class DataSingletonException : Exception;

/// <summary>
/// <para>This class is used to contains different Data Library in your Game</para>
/// <para> All the Class overriding <see cref="DataSingleton"/> are added through reflection in this Manager and opened through <see cref="GetDatas()"/> or <see cref="DataSingleton.Instance()"/> </para>
/// </summary>
public partial class GameDatasManager : Singleton<GameDatasManager>
{
    private Array<DataSingleton> _allManager = new();
    
    public static TManagerSubClass GetDatas<TManagerSubClass>() where TManagerSubClass : DataSingleton =>
        Instance._allManager._FindByPredicate(p_pManager => p_pManager is TManagerSubClass) as TManagerSubClass ??
            throw new DataSingletonException();
    
    public override void _Ready()
    {
        foreach (Type sGameDataManagerType in ReflectionHelper.GetEnumerableOfType<DataSingleton>())
            AddManager(sGameDataManagerType);
    }

    protected void AddManager<TType>() where TType : DataSingleton, new()
    {
        TType sType;
        AddChild(sType = new());
        _allManager.Add(sType);
    }
    
    protected void AddManager(Type p_type)
    {
        DataSingleton sType = (DataSingleton)Activator.CreateInstance(p_type)!;
        AddChild(sType);
        _allManager.Add(sType);
    }
}