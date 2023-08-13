using NomadsPlanet.Utils;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NomadsPlanet
{
    public class LeaderboardEntityDisplay : MonoBehaviour
    {
        public Sprite[] thumbs;

        [SerializeField] private Image displayThumb;
        [SerializeField] private TMP_Text displayText;

        public ulong ClientId { get; private set; }
        public int Coins { get; private set; }

        private FixedString32Bytes _playerName;
        private CharacterType _characterType;

        public void Initialize(ulong clientId, FixedString32Bytes playerName, CharacterType characterType, int coins)
        {
            ClientId = clientId;
            Coins = coins;
            _playerName = playerName;
            _characterType = characterType;

            UpdateCoins(Coins);
        }

        public void UpdateCoins(int coins)
        {
            Coins = coins;
            UpdateDisplays();
        }

        private void UpdateDisplays()
        {
            displayThumb.sprite = thumbs[(int)_characterType];
            displayText.text = $"1. {_playerName} ({Coins})";
        }
    }
}