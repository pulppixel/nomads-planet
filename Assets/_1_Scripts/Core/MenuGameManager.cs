using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using NomadsPlanet.Utils;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class MenuGameManager : MonoBehaviour
    {
        public Transform carParent;
        public Transform characterParent;

        public TMP_Text userNameText;
        public TMP_Text coinValueText;

        public CinemachineVirtualCamera virtualCamera;

        private readonly List<Transform> _carPrefabs = new();
        private readonly List<Transform> _characterPrefabs = new();

        private int _currentCar;
        private int _currentCharacter;

        private void Awake()
        {
            for (int i = 0; i < carParent.childCount; i++)
            {
                _carPrefabs.Add(carParent.GetChild(i));
            }

            for (int i = 0; i < characterParent.childCount; i++)
            {
                _characterPrefabs.Add(characterParent.GetChild(i));
            }
        }

        private void Start()
        {
            InitSetup();
        }

        public void SetLeftCarClick()
        {
            if (--_currentCar < 0)
            {
                _currentCar = _carPrefabs.Count - 1;
            }

            for (int i = 0; i < _carPrefabs.Count; i++)
            {
                _carPrefabs[i].gameObject.SetActive(i == _currentCar);
            }

            ES3.Save(PrefsKey.PlayerCarKey, (CarType)_currentCar);
        }

        public void SetRightCarClick()
        {
            if (++_currentCar >= _carPrefabs.Count)
            {
                _currentCar = 0;
            }

            for (int i = 0; i < _carPrefabs.Count; i++)
            {
                _carPrefabs[i].gameObject.SetActive(i == _currentCar);
            }

            ES3.Save(PrefsKey.PlayerCarKey, (CarType)_currentCar);
        }

        public void SetLeftCharacterClick()
        {
            if (--_currentCharacter < 0)
            {
                _currentCharacter = _characterPrefabs.Count - 1;
            }

            for (int i = 0; i < _characterPrefabs.Count; i++)
            {
                _characterPrefabs[i].gameObject.SetActive(i == _currentCharacter);
            }

            virtualCamera.Follow = _characterPrefabs[_currentCharacter];
            ES3.Save(PrefsKey.PlayerAvatarKey, (CharacterType)_currentCharacter);
            AllCharacterDefaultPosition();
        }

        public void SetRightCharacterClick()
        {
            if (++_currentCharacter >= _characterPrefabs.Count)
            {
                _currentCharacter = 0;
            }

            for (int i = 0; i < _characterPrefabs.Count; i++)
            {
                _characterPrefabs[i].gameObject.SetActive(i == _currentCharacter);
            }

            virtualCamera.Follow = _characterPrefabs[_currentCharacter];
            ES3.Save(PrefsKey.PlayerAvatarKey, (CharacterType)_currentCharacter);
            AllCharacterDefaultPosition();
        }

        private void AllCharacterDefaultPosition()
        {
            foreach (var character in _characterPrefabs)
            {
                character.localPosition = Vector3.zero;
                character.localRotation = Quaternion.identity;
            }
        }


        private void InitSetup()
        {
            _currentCar = (int)ES3.Load(PrefsKey.PlayerCarKey, CarType.Null);
            _currentCharacter = (int)ES3.Load(PrefsKey.PlayerAvatarKey, CharacterType.Null);
            userNameText.text = ES3.Load<string>(PrefsKey.PlayerNameKey);
            coinValueText.text = ES3.Load(PrefsKey.PlayerCoinKey, 0).ToString("N0");

            if (_currentCar == -1)
            {
                _currentCar = Random.Range(0, 8);
                ES3.Save(PrefsKey.PlayerCarKey, (CarType)_currentCar);
            }

            if (_currentCharacter == -1)
            {
                _currentCharacter = Random.Range(0, 8);
                ES3.Save(PrefsKey.PlayerAvatarKey, (CharacterType)_currentCharacter);
            }

            for (int i = 0; i < _carPrefabs.Count; i++)
            {
                _carPrefabs[i].gameObject.SetActive(i == _currentCar);
            }

            for (int i = 0; i < _characterPrefabs.Count; i++)
            {
                _characterPrefabs[i].gameObject.SetActive(i == _currentCharacter);
                _characterPrefabs[i].localRotation = Quaternion.identity;
            }

            virtualCamera.Follow = _characterPrefabs[_currentCharacter];
        }
    }
}