using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Debug
{
    public const string White = "FFFFFF";
    public const string Red = "FF3030";
    public const string Yellow = "FFFF00";
    public const string Green = "34FF3D";

    public static string LogText
    {
        get; private set;
    }

    public static void Log(string str, string color = White)
    {
        UnityEngine.Debug.Log(str);
        RegisterLog($"[LOG]{str}", color);
    }

    public static void LogError(string str)
    {
        UnityEngine.Debug.LogError(str);
        RegisterLog($"[ERR]{str}", Red);
    }

    public static void LogWarning(string str)
    {
        UnityEngine.Debug.LogWarning(str);
        RegisterLog($"[WAR]{str}", Yellow);
    }

    private static void RegisterLog(string str, string color = White)
    {
        LogText += $"<color=#{color}>{str}</color>\n";
    }

    public static void ClearLog()
    {
        LogText = "";
    }
}
