using Godot;

namespace ElementGodot.BaseGameLibrary.Helpers;

public static class TimString
{
    public static string _StripBBCode(this string p_input)
    {
        var regex = new RegEx();
        regex.Compile("\\[.+?\\]");
        return regex.Sub(p_input, "", true);
    }
}