using System.Collections;
using System.Threading.Tasks;
using NomadsPlanet.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NomadsPlanet
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private HostSingleton hostPrefab;
        [SerializeField] private NetworkObject playerPrefab;
#if UNITY_SERVER || UNITY_EDITOR
        [SerializeField] private ServerSingleton serverPrefab;
#endif

        private ApplicationData _appData;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);

            await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isDedicatedServer)
        {
            if (isDedicatedServer)
            {
                Application.targetFrameRate = 60;

                _appData = new ApplicationData();

#if UNITY_SERVER || UNITY_EDITOR
                ServerSingleton serverSingleton = Instantiate(serverPrefab);
                StartCoroutine(LoadGameSceneAsync(serverSingleton));
#endif
            }
            else
            {
                HostSingleton hostSingleton = Instantiate(hostPrefab);
                hostSingleton.CreateHost(playerPrefab);

                ClientSingleton clientSingleton = Instantiate(clientPrefab);
                bool authenticated = await clientSingleton.CreateClient();

                if (authenticated)
                {
                    clientSingleton.GameManager.GoToMenu();
                }
            }
        }

#if UNITY_SERVER || UNITY_EDITOR
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
#endif
    }
}