using Godot;

namespace ElementGodot.BaseGameLibrary.Datas;

public static class SingletonHelper
{
    public const string PATH_DATA = "res://Datas";
};


public partial class Singleton<TSelf> : Node where TSelf : class
{
    public static TSelf Instance { protected set; get; } = null!;

    public override void _Ready()
    {
        Singleton<TSelf>.Instance = (this as TSelf)!;
        if (!_LoadFromJson())
            _LoadFromResource();
    }

    protected virtual bool _LoadFromJson() => false;
    protected virtual bool _LoadFromResource() => false;
}

public partial class DataSingleton : Singleton<TSelf> where TSelf : class
{
}


public class RawSingleton<TSelf> where TSelf : class, new()
{
    private static TSelf _instance = null!;
    public static TSelf Instance => RawSingleton<TSelf>._instance ??= new TSelf();

    public RawSingleton() => RawSingleton<TSelf>._instance = (this as TSelf)!;
}