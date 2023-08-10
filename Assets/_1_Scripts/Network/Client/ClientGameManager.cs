using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

namespace NomadsPlanet.Client
{
    public class ClientGameManager
    {
        public async Task<bool> InitAsync()
        {
            // Authenticate Player
            await UnityServices.InitializeAsync();

            AuthState authState = await AuthenticationWrapper.DoAuth();

            return authState == AuthState.Authenticated;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(SceneName.MenuScene);
        }
    }
}