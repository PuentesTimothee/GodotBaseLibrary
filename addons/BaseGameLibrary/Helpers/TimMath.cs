namespace ElementGodot.BaseGameLibrary.Helpers;

public record struct BaseRange<TType>(TType _Min, TType _Max)
{
    public override string ToString()
    {
        return $"[{_Min}, {_Max}]";
    }
};