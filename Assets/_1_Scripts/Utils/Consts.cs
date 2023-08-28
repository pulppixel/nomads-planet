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
        public const string NameKey = "_NameKey3";
        public const string AvatarKey = "_AvatarKey3";
        public const string CarKey = "_CarKey3";
        public const string CoinKey = "_CoinKey3";
        public const string InGameCoinKey = "_InGameCoinKey3";
#else
        public const string NameKey = "NameKey3";
        public const string AvatarKey = "AvatarKey3";
        public const string CarKey = "CarKey3";
        public const string CoinKey = "CoinKey3";
        public const string InGameCoinKey = "InGameCoinKey3";
#endif
    }
}