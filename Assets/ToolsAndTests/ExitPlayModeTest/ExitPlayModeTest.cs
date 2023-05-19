using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPlayModeTest : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private bool asyncLoad = false;
    [SerializeField]
    private bool unityLoad = false;
    [SerializeField]
    private string sceneName = "2bl";
    [SerializeField]
    private bool loadInOnDisconneted = false;
    [SerializeField]
    private bool loadInOnLeftRoom = true;
    [SerializeField]
    private bool subscribeInAwake = false;
    [SerializeField]
    private bool unsusbcribeInOnDestroy = false;
    [SerializeField]
    private int callbacksWatcherToSpawn = 0;
    [SerializeField]
    private bool autoExit = false;

    #if UNITY_2018_1_OR_NEWER
    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
        Application.quitting += Quitting;
    }

    private static bool WantsToQuit()
    {
        Debug.LogFormat("ExitPlayModeTest.WantsToQuit (ConnectionHandler.AppQuits={0})", ConnectionHandler.AppQuits);
        return false;
    }

    private static void Quitting()
    {
        Debug.LogFormat("ExitPlayModeTest.Quitting (ConnectionHandler.AppQuits={0})", ConnectionHandler.AppQuits);
    }
    #endif

    private void Awake()
    {
        DontDestroyOnLoad(this);
        for (int i = 1; i <= this.callbacksWatcherToSpawn; i++)
        {
            GameObject go = new GameObject(string.Format("CallbacksWather #{0}", i));
            go.AddComponent<CallbackWatcher>();
        }
        if (this.subscribeInAwake)
        {
            PhotonNetwork.NetworkingClient.StateChanged += OnStateChanged;
            PhotonNetwork.AddCallbackTarget(this);
        }
    }

    public override void OnEnable()
    {
        if (!this.subscribeInAwake)
        {
            PhotonNetwork.NetworkingClient.StateChanged += OnStateChanged;
            base.OnEnable();
        }
    }
    
    private void OnStateChanged(ClientState fromState, ClientState toState)
    {
        if (toState == ClientState.Disconnected)
        {
            Debug.LogFormat("ExitPlayModeTest.OnStateChanged (ConnectionHandler.AppQuits={0}), from {1} to {2}", ConnectionHandler.AppQuits, fromState, toState);
        }
    }

    private void OnApplicationQuit()
    {
        Debug.LogFormat("ExitPlayModeTest.OnApplicationQuit (ConnectionHandler.AppQuits={0})", ConnectionHandler.AppQuits);
    }

    public override void OnDisable()
    {
        Debug.LogFormat("ExitPlayModeTest.OnDisable (ConnectionHandler.AppQuits={0})", ConnectionHandler.AppQuits);
        if (!this.unsusbcribeInOnDestroy)
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.StateChanged -= OnStateChanged;
        }
    }

    public override void OnJoinedRoom()
    {
        if (this.autoExit)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }

    private void OnDestroy()
    {
        Debug.LogFormat("ExitPlayModeTest.OnDestroy (ConnectionHandler.AppQuits={0})", ConnectionHandler.AppQuits);
        if (this.unsusbcribeInOnDestroy)
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.StateChanged -= OnStateChanged;
        }
    }

    public override void OnLeftRoom()
    {
        Debug.LogFormat("ExitPlayModeTest.OnLeftRoom (ConnectionHandler.AppQuits={0})", ConnectionHandler.AppQuits);
        if (this.loadInOnLeftRoom)
        {
            this.LoadScene();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogFormat("ExitPlayModeTest.OnDisconnected({0}) (ConnectionHandler.AppQuits={1})", cause, ConnectionHandler.AppQuits);
        if (this.loadInOnDisconneted)
        {
            this.LoadScene();
        }
    }

    
    private bool loading;

    private void LoadScene()
    {
        if (this.loading)
        {
            return;
        }
        this.loading = true;
        if (unityLoad)
        {
            if (asyncLoad)
            {
                SceneManager.LoadSceneAsync(this.sceneName);
            }
            else
            {
                SceneManager.LoadScene(this.sceneName);
                this.loading = false;
            }
        }
        else
        {
            PhotonNetwork.LoadLevel(this.sceneName);
        }
    }
}
