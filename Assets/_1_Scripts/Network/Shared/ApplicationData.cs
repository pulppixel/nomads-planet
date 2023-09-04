using System;
using System.Collections.Generic;
using System.Text;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    /// <summary>
    /// 기본 실행 명령 프로세서(멀티플레이는 IP와 포트 전달을 선호합니다)
    /// </summary>
    public class ApplicationData
    {
        /// <summary>
        /// Commands Dictionary
        /// 플래그 및 단일 변수 인수(예: '-argument', '-variableArg 변수')를 지원합니다.
        /// </summary>
        private readonly Dictionary<string, Action<string>> _commandDictionary = new();

        public static string IP()
        {
            return ES3.LoadString(PrefsKey.IPCmdKey, "127.0.0.1");
        }

        public static int Port()
        {
            return ES3.Load(PrefsKey.PortCmdKey, 7777);
        }

        public static int QPort()
        {
            return ES3.Load(PrefsKey.QueryPortCmdKey, 7787);
        }

        // 초기에 인스턴스화되도록 보장
        public ApplicationData()
        {
            SetIP("127.0.0.1");
            SetPort("7777");
            SetQueryPort("7787");
            _commandDictionary["-" + PrefsKey.IPCmdKey] = SetIP;
            _commandDictionary["-" + PrefsKey.PortCmdKey] = SetPort;
            _commandDictionary["-" + PrefsKey.QueryPortCmdKey] = SetQueryPort;
            ProcessCommandLineArguments(Environment.GetCommandLineArgs());
        }

        private void ProcessCommandLineArguments(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Launch Args: ");
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var nextArg = "";

                // 배열의 마지막 항목을 평가하는 경우, 해당 항목은 플래그여야 합니다.
                if (i + 1 < args.Length)
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
        /// 명령과 값은 쌍으로 배열된 args에 들어 있으므로
        /// </summary>
        private bool EvaluatedArgs(string arg, string nextArg)
        {
            if (!IsCommand(arg))
            {
                return false;
            }

            // 플래그가 필요한 경우 별도의 딕셔너리를 만드세요.
            if (IsCommand(nextArg))
            {
                return false;
            }

            _commandDictionary[arg].Invoke(nextArg);
            return true;
        }

        private static void SetIP(string ipArgument)
        {
            ES3.Save(PrefsKey.IPCmdKey, ipArgument);
        }

        private static void SetPort(string portArgument)
        {
            if (int.TryParse(portArgument, out int parsedPort))
            {
                ES3.Save(PrefsKey.PortCmdKey, parsedPort);
            }
            else
            {
                CustomFunc.ConsoleLog($"{portArgument}에 구문 분석 가능한 포트가 없습니다!");
            }
        }

        private static void SetQueryPort(string qPortArgument)
        {
            if (int.TryParse(qPortArgument, out int parsedQPort))
            {
                ES3.Save(PrefsKey.QueryPortCmdKey, parsedQPort);
            }
            else
            {
                CustomFunc.ConsoleLog($"{qPortArgument}에 구문 분석 가능한 쿼리 포트가 포함되어 있지 않습니다!");
            }
        }

        private bool IsCommand(string arg)
        {
            return !string.IsNullOrEmpty(arg) && _commandDictionary.ContainsKey(arg) && arg.StartsWith("-");
        }
    }
}