using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using NomadsPlanet.Utils;
using UnityEngine.UI;
using VivoxUnity;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class MenuGameManager : MonoBehaviour
    {
        [SerializeField] private Transform carParent;
        [SerializeField] private Transform characterParent;
        [SerializeField] private Transform characterStartPoint;

        [SerializeField] private TMP_Text userNameText;
        [SerializeField] private TMP_Text coinValueText;

        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private LoadingFaderController faderController;

        [SerializeField] private Image connectionIndicator;
        [SerializeField] private TMP_Text connectionText;

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
            
            VivoxVoiceManager.Instance.OnUserLoggedInEvent += OnUserLoggedIn;
            VivoxVoiceManager.Instance.OnUserLoggedOutEvent += OnUserLoggedOut;
            VivoxVoiceManager.Instance.OnRecoveryStateChangedEvent += OnRecoveryStateChanged;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(.1f);

            InitSetup();
            bgmSource.volume = 0f;
            bgmSource.DOFade(1f, .5f);
            StartCoroutine(faderController.FadeOut());

            VivoxVoiceManager.Instance.Login(ES3.LoadString(PrefsKey.NameKey, "Unknown"));
            yield return new WaitUntil(() => VivoxVoiceManager.Instance.LoginState == LoginState.LoggedIn);

            OnUserLoggedIn();
        }

        private static void JoinChannel()
        {
            VivoxVoiceManager.Instance.OnParticipantAddedEvent += VivoxVoiceManager_OnParticipantAddedEvent;
            VivoxVoiceManager.Instance.JoinChannel(SceneName.MenuScene, ChannelType.NonPositional,
                VivoxVoiceManager.ChatCapability.TextOnly);
        }

        private static void VivoxVoiceManager_OnParticipantAddedEvent(string username, ChannelId channel,
            IParticipant participant)
        {
            if (channel.Name == SceneName.MenuScene && participant.IsSelf)
            {
                // if joined the lobby channel and we're not hosting a match
                // we should request invites from hosts
            }
        }

        private static void OnUserLoggedIn()
        {
            var lobbyChannel =
                VivoxVoiceManager.Instance.ActiveChannels.FirstOrDefault(ac => ac.Channel.Name == SceneName.MenuScene);

            if (VivoxVoiceManager.Instance && VivoxVoiceManager.Instance.ActiveChannels.Count == 0 ||
                lobbyChannel == null)
            {
                JoinChannel();
            }
            else
            {
                CustomFunc.ConsoleLog("Now Transmitting into lobby channel");
            }
        }

        private static void OnUserLoggedOut()
        {
            VivoxVoiceManager.Instance.DisconnectAllChannels();
        }

        private void OnRecoveryStateChanged(ConnectionRecoveryState state)
        {
            connectionIndicator.color = state switch
            {
                ConnectionRecoveryState.Connected => Color.green,
                ConnectionRecoveryState.Disconnected => Color.red,
                ConnectionRecoveryState.FailedToRecover => Color.black,
                ConnectionRecoveryState.Recovered => Color.green,
                ConnectionRecoveryState.Recovering => Color.yellow,
                _ => Color.white
            };

            connectionText.text = state.ToString();
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

                if (i == _currentCharacter)
                {
                    _characterPrefabs[i].transform.position = characterStartPoint.position;
                }
            }
            
            virtualCamera.Follow = _characterPrefabs[_currentCharacter];
        }

        private void OnDestroy()
        {
            VivoxVoiceManager.Instance.OnUserLoggedInEvent -= OnUserLoggedIn;
            VivoxVoiceManager.Instance.OnUserLoggedOutEvent -= OnUserLoggedOut;
            VivoxVoiceManager.Instance.OnParticipantAddedEvent -= VivoxVoiceManager_OnParticipantAddedEvent;
            VivoxVoiceManager.Instance.OnRecoveryStateChangedEvent -= OnRecoveryStateChanged;
        }
    }
}