using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class TeamChooser : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private GameObject buttonsHolder;
    [SerializeField]
    private Text myTeamText;

    private void Start()
    {
        this.InitTeam("Red");
        this.InstantiateButtons();
    }

    private void Update()
    {
        if (this.buttonsHolder.activeSelf != PhotonNetwork.InRoom)
        {
            this.buttonsHolder.SetActive(PhotonNetwork.InRoom);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonTeamsManager.PlayerLeftTeam += this.OnPlayerLeftTeam;
        PhotonTeamsManager.PlayerJoinedTeam += this.OnPlayerJoinedTeam;
    }

    private void OnPlayerLeftTeam(Player player, PhotonTeam team)
    {
        Debug.LogFormat("Player {0} left team {1}", player, team);
        if (player.IsLocal)
        {
            this.myTeamText.text = "No Team";
        }
    }

    private void OnPlayerJoinedTeam(Player player, PhotonTeam team)
    {
        Debug.LogFormat("Player {0} joined team {1}", player, team);
        if (player.IsLocal)
        {
            this.myTeamText.text = team.Name;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonTeamsManager.PlayerLeftTeam -= this.OnPlayerLeftTeam;
        PhotonTeamsManager.PlayerJoinedTeam -= this.OnPlayerJoinedTeam;
    }

    private void InstantiateButtons()
    {
        PhotonTeam[] availableTeams = PhotonTeamsManager.Instance.GetAvailableTeams();
        this.InstantiateLeaveButton();
        for (int i = 0; i < availableTeams.Length; i++)
        {
            this.InstantiateTeamButton(availableTeams[i], false);
            this.InstantiateTeamButton(availableTeams[i], true);
        }
    }

    private void InstantiateLeaveButton()
    {
        GameObject go = Instantiate(this.buttonPrefab, Vector3.zero, Quaternion.identity, this.buttonsHolder.transform);
        Text txt = go.GetComponentInChildren<Text>();
        txt.text = "LeaveCurrentTeam";
        go.name = "LeaveCurrentTeam";
        Button btn = go.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => { PhotonNetwork.LocalPlayer.LeaveCurrentTeam(); });
    }

    private void InstantiateTeamButton(PhotonTeam team, bool join)
    {
        GameObject go = Instantiate(this.buttonPrefab, Vector3.zero, Quaternion.identity, this.buttonsHolder.transform);
        Button btn = go.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        if (join)
        {
            go.name = string.Format("Join {0}", team);
            btn.onClick.AddListener(() => { PhotonNetwork.LocalPlayer.JoinTeam(team); });
        }
        else
        {
            go.name = string.Format("Switch to {0}", team);
            btn.onClick.AddListener(() => { PhotonNetwork.LocalPlayer.SwitchTeam(team); });
        }
        Text txt = go.GetComponentInChildren<Text>();
        txt.text = go.name;
    }

    public override void OnJoinedRoom()
    {
        PhotonTeam team = PhotonNetwork.LocalPlayer.GetPhotonTeam();
        if (team == null)
        {
            this.myTeamText.text = "No Team";
        }
        else
        {
            this.myTeamText.text = team.Name;
        }
    }

    private void InitTeam(string teamName)
    {
        bool result = PhotonNetwork.LocalPlayer.JoinTeam(teamName);
        Debug.LogFormat("PhotonNetwork.LocalPlayer.JoinTeam(\"{0}\") result {1}", teamName, result);
        if (result)
        {
            this.myTeamText.text = teamName;
        }
        else
        {
            this.myTeamText.text = "No Team";
        }
    }
}
