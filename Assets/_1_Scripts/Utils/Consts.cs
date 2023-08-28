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
        public const string NameKey = "_NameKey2";
        public const string AvatarKey = "_AvatarKey2";
        public const string CarKey = "_CarKey2";
        public const string CoinKey = "_CoinKey2";
        public const string InGameCoinKey = "_InGameCoinKey2";
#else
        public const string NameKey = "NameKey2";
        public const string AvatarKey = "AvatarKey2";
        public const string CarKey = "CarKey2";
        public const string CoinKey = "CoinKey2";
        public const string InGameCoinKey = "InGameCoinKey2";
#endif
    }
}