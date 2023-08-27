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
        public const string PlayerNameKey = "_PlayerName";
        public const string PlayerAvatarKey = "_PlayerAvatar";
        public const string PlayerCarKey = "_PlayerCar";
        public const string PlayerCoinKey = "_PlayerCoin";
        public const string LocalCoinKey = "_LocalCoin";
#else
        public const string PlayerNameKey = "PlayerName";
        public const string PlayerAvatarKey = "PlayerAvatar";
        public const string PlayerCarKey = "PlayerCar";
        public const string PlayerCoinKey = "PlayerCoin";
        public const string LocalCoinKey = "LocalCoin";
#endif
    }
}