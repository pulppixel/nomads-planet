namespace NomadsPlanet.Utils
{
    public struct SceneName
    {
        public const string CharacterEditor = "_CharacterEditor";
        public const string GameScene = "_GameScene";
        public const string MainScene = "_MainScene";
        public const string MenuScene = "_MenuScene";
        public const string NetBootStrap = "_NetBootStrap";
    }

    public struct NetworkSetup
    {
        public const int MaxConnections = 20;
        public const string ConnectType = "dtls"; // "udp";
    }
}