using System;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    [Serializable]
    public class UserData
    {
        public string userName;
        public string userAuthId;
        public CharacterType userAvatarType;
        public GameInfo userGamePreferences;
    }

    [Serializable]
    public class GameInfo
    {
        public Map map;
        public GameMode gameMode;

        public string ToMultiplayQueue()
        {
            return "";
        }
    }
}