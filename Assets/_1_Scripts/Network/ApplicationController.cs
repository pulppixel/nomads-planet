using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using System.Collections;
using UnityEngine.SceneManagement;

namespace NomadsPlanet
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private HostSingleton hostPrefab;
        [SerializeField] private ServerSingleton serverPrefab;
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private VivoxVoiceManager vivoxPrefab;

        private ApplicationData _appData;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);

            await LaunchInMode();
        }

        private async Task LaunchInMode()
        {
#if UNITY_SERVER
            Application.targetFrameRate = 60;

            _appData = new ApplicationData();

            ServerSingleton serverSingleton = Instantiate(serverPrefab);

            StartCoroutine(LoadGameSceneAsync(serverSingleton));
#else
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(playerPrefab);

            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated)
            {
                ClientGameManager.GoToMenu();
            }
            
            VivoxVoiceManager vivoxSingleton = Instantiate(vivoxPrefab);
            vivoxSingleton.Login(ES3.LoadString(PrefsKey.NameKey, "Unknown"));
#endif
        }

        private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneName.GameScene);

            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            Task createServerTask = serverSingleton.CreateServer(playerPrefab);
            yield return new WaitUntil(() => createServerTask.IsCompleted);

            Task startServerTask = serverSingleton.GameManager.StartGameServerAsync();
            yield return new WaitUntil(() => startServerTask.IsCompleted);
        }
    }
}