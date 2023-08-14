using System;
using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private ResultBoard resultBoard;

        private void Start()
        {
            Cursor.visible = false;
        }

        public void OpenResultBoard()
        {
            Cursor.visible = true;
            resultBoard.gameObject.SetActive(true);
            resultBoard.Entrance();
        }

        public void LeaveGame()
        {
            // todo: 게임 결과창 띄우기
            if (NetworkManager.Singleton.IsHost)
            {
                HostSingleton.Instance.GameManager.Shutdown();
            }

            ClientSingleton.Instance.GameManager.Disconnect();
        }
    }
}