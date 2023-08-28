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
        public const string ConnectType = "udp"; // "dtls";
        public const string JoinCode = "JoinCode";
        public const string SoloQueue = "solo-queue";
        // public const string TeamQueue = "team-queue";
    }

    public struct PrefsKey
    {
#if UNITY_EDITOR
        public const string NameKey = "_NameKey";
        public const string AvatarKey = "_AvatarKey";
        public const string CarKey = "_CarKey";
        public const string CoinKey = "_CoinKey";
        public const string InGameCoinKey = "_InGameCoinKey";
#else
        public const string NameKey = "NameKey";
        public const string AvatarKey = "AvatarKey";
        public const string CarKey = "CarKey";
        public const string CoinKey = "CoinKey";
        public const string InGameCoinKey = "InGameCoinKey";
#endif
    }
}