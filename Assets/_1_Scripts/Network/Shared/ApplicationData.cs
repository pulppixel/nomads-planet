using System;
using System.Collections.Generic;
using System.Text;
using NomadsPlanet.Utils;

/// <summary>
/// Basic launch command processor (Multiplay prefers passing IP and port along)
/// </summary>
public class ApplicationData
{
    /// <summary>
    /// Commands Dictionary
    /// Supports flags and single variable args (eg. '-argument', '-variableArg variable')
    /// </summary>
    private readonly Dictionary<string, Action<string>> _commandDictionary = new();

    private const string IPCmdKey = "k_ip";
    private const string PortCmdKey = "k_port";
    private const string QueryPortCmdKey = "k_queryPort";

    public static string IP()
    {
        return ES3.LoadString(IPCmdKey, "127.0.0.1");
    }

    public static int Port()
    {
        return ES3.Load(PortCmdKey, 7777);
    }

    public static int QPort()
    {
        return ES3.Load(QueryPortCmdKey, 7787);
    }

    //Ensure this gets instantiated Early on
    public ApplicationData()
    {
        SetIP("127.0.0.1");
        SetPort("7777");
        SetQueryPort("7787");
        _commandDictionary["-" + IPCmdKey] = SetIP;
        _commandDictionary["-" + PortCmdKey] = SetPort;
        _commandDictionary["-" + QueryPortCmdKey] = SetQueryPort;
        ProcessCommandLinearguments(Environment.GetCommandLineArgs());
    }

    private void ProcessCommandLinearguments(string[] args)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Launch Args: ");
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var nextArg = "";

            if (i + 1 < args.Length) // if we are evaluating the last item in the array, it must be a flag
            {
                nextArg = args[i + 1];
            }

            if (EvaluatedArgs(arg, nextArg))
            {
                sb.Append(arg);
                sb.Append(" : ");
                sb.AppendLine(nextArg);
                i++;
            }
        }

        CustomFunc.ConsoleLog(sb);
    }

    /// <summary>
    /// Commands and values come in the args array in pairs, so we
    /// </summary>
    private bool EvaluatedArgs(string arg, string nextArg)
    {
        if (!IsCommand(arg))
        {
            return false;
        }
        
        if (IsCommand(nextArg)) // If you have need for flags, make a separate dict for those.
        {
            return false;
        }

        _commandDictionary[arg].Invoke(nextArg);
        return true;
    }

    private static void SetIP(string ipArgument)
    {
        ES3.Save(IPCmdKey, ipArgument);
    }

    private static void SetPort(string portArgument)
    {
        if (int.TryParse(portArgument, out int parsedPort))
        {
            ES3.Save(PortCmdKey, parsedPort);
        }
        else
        {
            CustomFunc.ConsoleLog($"{portArgument} does not contain a parseable port!");
        }
    }

    private static void SetQueryPort(string qPortArgument)
    {
        if (int.TryParse(qPortArgument, out int parsedQPort))
        {
            ES3.Save(QueryPortCmdKey, parsedQPort);
        }
        else
        {
            CustomFunc.ConsoleLog($"{qPortArgument} does not contain a parseable query port!");
        }
    }

    private bool IsCommand(string arg)
    {
        return !string.IsNullOrEmpty(arg) && _commandDictionary.ContainsKey(arg) && arg.StartsWith("-");
    }
}