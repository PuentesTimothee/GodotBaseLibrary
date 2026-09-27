using Godot;
using Godot.Collections;

namespace ElementGodot.BaseGameLibrary.Helpers;


public enum LogSeverity
{
    e_Default,
    e_Verbose,
    e_VeryVerbose
}

public enum LogType
{
    e_Default,
    e_Warning,
    e_Error
}

/// <summary>
/// <para><b>Helpers class to log different infos inside the Godot Log system.</b></para>
/// <para>  Multiple <see cref="LogType"/> are present and different <see cref="LogSeverity"/>. Each are configurable with the exe parameters. The parameters must be at least of a Severity Greater than the setting to be displayed (e_Default->e_Verbose->e_VeryVerbose)</para>
/// <para> Possible Options: <br/>
/// "--log=" -> <see cref="LogType.e_Default"/> <br/>
/// "--logW=" -> <see cref="LogType.e_Warning"/> <br/>
/// "--logE=" -> <see cref="LogType.e_Error"/> <br/>
/// </para>
/// <para> Possible values: <br/>
/// Default -> <see cref="LogSeverity.e_Default"/> <br/>
/// Verbose -> <see cref="LogSeverity.e_Verbose"/> <br/>
/// VeryVerbose -> <see cref="LogSeverity.e_VeryVerbose"/> <br/>
/// </para>
/// </summary>
public sealed class MyLogger
{
    private static readonly MyLogger _instance = new();
    private System.Collections.Generic.Dictionary<LogType, LogSeverity> _currentSeverity = new();

    private MyLogger()
    {
        var args = new Array<string>(OS.GetCmdlineUserArgs());
        System.Collections.Generic.Dictionary<string, LogType> sValueString = new();
        sValueString.Add("--log=", LogType.e_Default);
        sValueString.Add("--logW=", LogType.e_Warning);
        sValueString.Add("--logE=", LogType.e_Error);

        foreach (string arg in args)
        {
            foreach (var (str, val) in sValueString)
            {
                if (arg.StartsWith(str))
                    if (TimEnums.TryParseWithPrefix(arg.Substring(str.Length + 1), out LogSeverity logType))
                        MyLogger._SetSeverityFor(val, logType);
            }
        }
    }
        
    private static void _SetSeverityFor(LogType p_type, LogSeverity p_severity) =>
        MyLogger._instance._currentSeverity.TryAdd(p_type, p_severity);
	
    private static LogSeverity _SeverityFor(LogType p_type)
    {
        if (MyLogger._instance._currentSeverity.TryGetValue(p_type, out LogSeverity ret))
            return ret;
        return LogSeverity.e_Default;
    }
		
    /// <inheritdoc cref="MyLogger"/>
    public static void _LogErrorCommon(string p_message) => MyLogger._Log(LogSeverity.e_Default, LogType.e_Error, p_message);
    /// <inheritdoc cref="MyLogger"/>
    public static void _LogError(LogSeverity p_severity, string p_message) => MyLogger._Log(p_severity, LogType.e_Error, p_message);

    /// <inheritdoc cref="MyLogger"/>
    public static void _LogWarning(LogSeverity p_severity, string p_message) => MyLogger._Log(p_severity, LogType.e_Warning, p_message);

    /// <inheritdoc cref="MyLogger"/>
    public static void _LogTextCommon(string p_message) => MyLogger._LogText(LogSeverity.e_Default, p_message);

    /// <inheritdoc cref="MyLogger"/>
    public static void _LogText(LogSeverity p_severity, string p_message) => MyLogger._Log(p_severity, LogType.e_Default, p_message);

    /// <inheritdoc cref="MyLogger"/>
    public static void _Log(LogSeverity p_severity, LogType p_type, string p_message)
    {
        if (MyLogger._SeverityFor(p_type) >= p_severity)
            switch (p_type)
            {
                case LogType.e_Default:
                    GD.Print(p_message);
                    return;
                case LogType.e_Warning:
                    GD.PushWarning(p_message);
                    return;
                case LogType.e_Error:
                    GD.PushError(p_message);
                    return;
            }
    }
};