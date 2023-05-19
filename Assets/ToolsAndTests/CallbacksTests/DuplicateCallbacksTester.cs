using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class DuplicateCallbacksTester : MonoBehaviour
{
    [SerializeField]
    private BaseCallbacks[] callbacksComponents;

    [SerializeField] 
    private Button toggleConnectionButton;

    private Text toggleConnectionText;

    public void ToggleCallbacksComponents()
    {
        for (int i = 0; i < callbacksComponents.Length; i++)
        {
            callbacksComponents[i].enabled = !callbacksComponents[i].enabled;
        }
    }

    private void Awake()
    {
        Refresh();
        if (toggleConnectionButton)
        {
            toggleConnectionText = toggleConnectionButton.GetComponentInChildren<Text>();
        }
    }

    public void Refresh()
    {
        callbacksComponents = GetComponentsInChildren<BaseCallbacks>(true);
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void ToggleConnectButton()
    {
        if (!toggleConnectionText && toggleConnectionButton)
        {
            toggleConnectionText = toggleConnectionButton.GetComponentInChildren<Text>();
        }
        switch (PhotonNetwork.NetworkClientState)
        {
            case ClientState.Disconnected:
            case ClientState.PeerCreated:
                Connect();
                if (toggleConnectionText)
                {
                    toggleConnectionText.text = "Disconnect";
                }
                break;
            default:
                Disconnect();
                if (toggleConnectionText)
                {
                    toggleConnectionText.text = "Connect";
                }
                break;
        }
    }
}
