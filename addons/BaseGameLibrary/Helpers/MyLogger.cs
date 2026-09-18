using System.Collections.Generic;
using Godot;

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

public sealed class MyLogger
{
    private static readonly MyLogger _instance = new();
    private Dictionary<LogType, LogSeverity> _currentSeverity = new();

    public static void _SetSeverityFor(LogType p_type, LogSeverity p_severity) =>
        MyLogger._instance._currentSeverity.TryAdd(p_type, p_severity);
	
    private static LogSeverity _SeverityFor(LogType p_type)
    {
        if (MyLogger._instance._currentSeverity.TryGetValue(p_type, out LogSeverity ret))
            return ret;
        return LogSeverity.e_Default;
    }
		
    public static void _LogErrorCommon(string p_message) => MyLogger._Log(LogSeverity.e_Default, LogType.e_Error, p_message);
    public static void _LogError(LogSeverity p_severity, string p_message) => MyLogger._Log(p_severity, LogType.e_Error, p_message);

    public static void _LogWarning(LogSeverity p_severity, string p_message) => MyLogger._Log(p_severity, LogType.e_Warning, p_message);

    public static void _LogTextCommon(string p_message) => MyLogger._LogText(LogSeverity.e_Default, p_message);

    public static void _LogText(LogSeverity p_severity, string p_message) => MyLogger._Log(p_severity, LogType.e_Default, p_message);

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