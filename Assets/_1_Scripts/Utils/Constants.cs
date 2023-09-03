namespace NomadsPlanet.Utils
{
    public struct SceneName
    {
        // public const string BootStrap = "_BootStrap";
        public const string NetBootStrap = "_NetBootStrap";
        public const string GameScene = "_GameScene";
        public const string MenuScene = "_MenuScene";
    }

    public struct NetworkSetup
    {
        public const int MaxConnections = 20;
        public const string ConnectType = "dtls"; // "udp";
        public const string JoinCode = "JoinCode";
        public const string SoloQueue = "solo-queue";
        public const string TeamQueue = "team-queue";
    }

    public struct PrefsKey
    {
#if UNITY_EDITOR
        public const string IPCmdKey = "ip";
        public const string PortCmdKey = "port";
        public const string QueryPortCmdKey = "queryPort";

        public const string NameKey = "_Name_Key_3";
        public const string AvatarTypeKey = "_Avatar_Key_3";
        public const string CarTypeKey = "_Car_Key_3";
        public const string CoinKey = "_Coin_Key_3";
#else
        public const string IPCmdKey = "ip";
        public const string PortCmdKey = "port";
        public const string QueryPortCmdKey = "queryPort";
        
        public const string NameKey = "Name_Key_3";
        public const string AvatarTypeKey = "Avatar_Key_3";
        public const string CarTypeKey = "Car_Key_3";
        public const string CoinKey = "Coin_Key_3";
#endif
    }
}