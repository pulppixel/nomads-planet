namespace NomadsPlanet.Utils
{
    public struct SceneName
    {
        public const string BootStrap = "_BootStrap";
        public const string NetBootStrap = "_NetBootStrap";
        public const string CharacterEditor = "_CharacterEditor";
        public const string GameScene = "_GameScene";
        public const string MainScene = "_MainScene";
        public const string MenuScene = "_MenuScene";
    }

    public struct NetworkSetup
    {
        public const int MaxConnections = 20;
        public const string ConnectType = "dtls"; // "udp";
        public const string JoinCode = "JoinCode";
        public const string LobbyName = "My Lobby";
    }

    public struct PrefsKey
    {
#if UNITY_EDITOR
        public const string PlayerNameKey = "_PlayerName";
        public const string PlayerAvatarKey = "_PlayerAvatar";
#else
        public const string PlayerNameKey = "PlayerName";
        public const string PlayerAvatarKey = "PlayerAvatar";
#endif
    }
}