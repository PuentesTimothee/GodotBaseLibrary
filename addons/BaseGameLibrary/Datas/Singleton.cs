using Godot;

namespace ElementGodot.BaseGameLibrary.Datas;

public static class SingletonHelper
{
    public const string PATH_DATA = "res://Datas";
};


public abstract partial class DataNode : Node
{
    public override void _Ready()
    {
        if (!_LoadFromJson())
            _LoadFromResource();
    }

    protected virtual bool _LoadFromJson() => false;
    protected virtual bool _LoadFromResource() => false;
};

public abstract partial class Singleton<TSelf> : DataNode where TSelf : class
{
    public static TSelf Instance { protected set; get; } = null!;

    public override void _Ready()
    {
        Singleton<TSelf>.Instance = (this as TSelf)!;
        base._Ready();
    }
}

/// <summary>
/// <para>This class is used to represent different Data Library in your Game</para>
/// <para>ny class overriden this is added to the <see cref="Datas.GameDatasManager"/></para>
/// </summary>
public abstract partial class DataSingleton : DataNode
{
    public static TSelf Instance<TSelf>() where TSelf : DataSingleton => GameDatasManager.GetDatas<TSelf>();
}

public class RawSingleton<TSelf> where TSelf : class, new()
{
    private static TSelf _instance = null!;
    public static TSelf Instance => RawSingleton<TSelf>._instance ??= new TSelf();

    public RawSingleton() => RawSingleton<TSelf>._instance = (this as TSelf)!;
}