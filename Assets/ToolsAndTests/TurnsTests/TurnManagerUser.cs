using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class TurnManagerUser : MonoBehaviour, IPunTurnManagerCallbacks
{
    public PunTurnManager turnManager;

    void Start()
    {
        
        this.turnManager = this.gameObject.GetComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;
        
        // duration of the turn
        turnManager.TurnDuration = 10f;
    }

    public void GameBegins()
    {
        Debug.Log("Game Started.");
        Debug.Log("Turn " + turnManager.Turn);
        if (PhotonNetwork.IsMasterClient)
        {
            turnManager.BeginTurn();
            Debug.Log(PhotonNetwork.CurrentRoom.GetTurn());
        }
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            this.turnManager.BeginTurn();
        }
    }
    /// <inheritdoc />
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins: " + turn + " room turn: " +PhotonNetwork.CurrentRoom.GetTurn());
    }

    /// <inheritdoc />
    public void OnTurnCompleted(int turn)
    {
        Debug.Log("Callback called");
    }

    /// <inheritdoc />
    public void OnPlayerMove(Player player, int turn, object move)
    {
        Debug.Log("Callback called");
    }

    /// <inheritdoc />
    public void OnPlayerFinished(Player player, int turn, object move)
    {
        Debug.Log("Callback called");
    }

    /// <inheritdoc />
    public void OnTurnTimeEnds(int turn)
    {
        Debug.Log("OnTurnTimeEnds: " + turn + " room turn: " +PhotonNetwork.CurrentRoom.GetTurn());
    }
}
