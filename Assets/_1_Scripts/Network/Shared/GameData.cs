using System;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    [Serializable]
    public class UserData
    {
        public string userName;
        public string userAuthId;
        public int userCarType;
        public int userAvatarType;
        public GameInfo userGamePreferences = new();
    }

    [Serializable]
    public class GameInfo
    {
        public Map map;
        public GameMode gameMode;
        public GameQueue gameQueue;

        public string ToMultiplayQueue()
        {
            return gameQueue switch
            {
                GameQueue.Solo => NetworkSetup.SoloQueue,
                // GameQueue.Team => NetworkSetup.TeamQueue,
                _ => NetworkSetup.SoloQueue,
            };
        }
    }
}