using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using NomadsPlanet.Utils;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class MenuGameManager : MonoBehaviour
    {
        [SerializeField] private Transform carParent;
        [SerializeField] private Transform characterParent;

        [SerializeField] private TMP_Text userNameText;
        [SerializeField] private TMP_Text coinValueText;

        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private LoadingFaderController faderController;

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

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(.1f);

            InitSetup();
            bgmSource.volume = 0f;
            bgmSource.DOFade(1f, .5f);
            StartCoroutine(faderController.FadeOut());
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

            ES3.Save(PrefsKey.CarTypeKey, _currentCar);
            ClientSingleton.Instance.GameManager.UpdateUserData(userCarType: _currentCar);
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

            ES3.Save(PrefsKey.CarTypeKey, _currentCar);
            ClientSingleton.Instance.GameManager.UpdateUserData(userCarType: _currentCar);
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
            ES3.Save(PrefsKey.AvatarTypeKey, _currentCharacter);
            ClientSingleton.Instance.GameManager.UpdateUserData(userAvatarType: _currentCharacter);
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
            ES3.Save(PrefsKey.AvatarTypeKey, _currentCharacter);
            ClientSingleton.Instance.GameManager.UpdateUserData(userAvatarType: _currentCharacter);
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
            _currentCar = ES3.Load(PrefsKey.CarTypeKey, -1);
            _currentCharacter = ES3.Load(PrefsKey.AvatarTypeKey, -1);

            if (_currentCar == -1)
            {
                _currentCar = Random.Range(0, 8);
                ES3.Save(PrefsKey.CarTypeKey, _currentCar);
                ClientSingleton.Instance.GameManager.UpdateUserData(userCarType: _currentCar);
            }

            if (_currentCharacter == -1)
            {
                _currentCharacter = Random.Range(0, 8);
                ES3.Save(PrefsKey.AvatarTypeKey, _currentCharacter);
                ClientSingleton.Instance.GameManager.UpdateUserData(userAvatarType: _currentCharacter);
            }

            userNameText.text = ES3.LoadString(PrefsKey.NameKey, "Unknown");
            coinValueText.text = ES3.Load(PrefsKey.CoinKey, 0).ToString("N0");

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