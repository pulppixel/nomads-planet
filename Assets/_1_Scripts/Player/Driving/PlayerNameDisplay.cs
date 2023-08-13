using TMPro;
using Unity.Collections;
using UnityEngine;

namespace NomadsPlanet
{
    public class PlayerNameDisplay : MonoBehaviour
    {
        [SerializeField] private PlayerSetter player;
        [SerializeField] private TMP_Text playerNameText;

        private void Start()
        {
            HandlePlayerNameChanged(string.Empty, player.playerName.Value);
            player.playerName.OnValueChanged += HandlePlayerNameChanged;
        }

        private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
        {
            playerNameText.text = newName.ToString();
        }

        private void OnDestroy()
        {
            player.playerName.OnValueChanged -= HandlePlayerNameChanged;
        }
    }
}