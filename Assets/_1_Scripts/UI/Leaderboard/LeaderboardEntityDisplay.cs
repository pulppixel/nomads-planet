using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    public class LeaderboardEntityDisplay : MonoBehaviour
    {
        public Sprite[] thumbs;

        [SerializeField] private Image displayThumb;
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private Color myColor;

        public ulong ClientId { get; private set; }
        public int Coins { get; private set; }

        private FixedString32Bytes _playerName;
        private int _characterType;

        public void Initialize(ulong clientId, FixedString32Bytes playerName, int characterType, int coins)
        {
            ClientId = clientId;
            Coins = coins;
            _playerName = playerName;
            _characterType = characterType;

            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                displayText.color = myColor;
            }

            UpdateCoins(Coins);
        }

        public void UpdateCoins(int coins)
        {
            Coins = coins;
            UpdateDisplays();
        }

        public void UpdateDisplays()
        {
            displayThumb.sprite = thumbs[_characterType];
            displayText.text =
                $"{_playerName.ToString().TruncateString(9)} " +
                $"({Coins})";
        }
    }
}