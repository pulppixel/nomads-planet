using TMPro;
using Unity.Collections;
using UnityEngine;

namespace NomadsPlanet
{
    public class PlayerNameDisplay : MonoBehaviour
    {
        [SerializeField] private DrivingPlayer drivingPlayer;
        [SerializeField] private TMP_Text playerNameText;

        private void Start()
        {
            HandlePlayerNameChanged(string.Empty, drivingPlayer.playerName.Value);
            drivingPlayer.playerName.OnValueChanged += HandlePlayerNameChanged;
        }

        private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
        {
            playerNameText.text = newName.ToString();
        }

        private void OnDestroy()
        {
            drivingPlayer.playerName.OnValueChanged -= HandlePlayerNameChanged;
        }
    }
}