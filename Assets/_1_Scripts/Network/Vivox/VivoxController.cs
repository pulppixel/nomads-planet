using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using NomadsPlanet.Utils;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;

namespace NomadsPlanet
{
    public class VivoxController : MonoBehaviour
    {
        [SerializeField] private Button chatButton;
        [SerializeField] private Image connectionIndicator;
        [SerializeField] private RectTransform chatRoomRectTr;

        private void Awake()
        {
#if UNITY_SERVER
            chatButton.gameObject.SetActive(false);
            connectionIndicator.gameObject.SetActive(false);
#else
            VivoxVoiceManager.Instance.OnUserLoggedInEvent += OnUserLoggedIn;
            VivoxVoiceManager.Instance.OnUserLoggedOutEvent += OnUserLoggedOut;
            VivoxVoiceManager.Instance.OnRecoveryStateChangedEvent += OnRecoveryStateChanged;
#endif
        }

        private IEnumerator Start()
        {
            chatRoomRectTr.localScale = new Vector3(1f, 0f, 1f);
#if !UNITY_SERVER
            yield return new WaitForSeconds(2f);
            VivoxVoiceManager.Instance.Login(ES3.LoadString(PrefsKey.NameKey, "Unknown"));
            yield return new WaitUntil(() => VivoxVoiceManager.Instance.LoginState == LoginState.LoggedIn);

            OnUserLoggedIn();
#endif
        }

        private void OnDestroy()
        {
#if !UNITY_SERVER
            VivoxVoiceManager.Instance.OnUserLoggedInEvent -= OnUserLoggedIn;
            VivoxVoiceManager.Instance.OnUserLoggedOutEvent -= OnUserLoggedOut;
            VivoxVoiceManager.Instance.OnRecoveryStateChangedEvent -= OnRecoveryStateChanged;
#endif
        }

        private static void JoinChannel()
        {
            VivoxVoiceManager.Instance.OnParticipantAddedEvent += VivoxVoiceManager_OnParticipantAddedEvent;
            VivoxVoiceManager.Instance.JoinChannel(HostSingleton.Instance.GameManager.JoinCode,
                ChannelType.NonPositional,
                VivoxVoiceManager.ChatCapability.TextAndAudio);
        }

        private static void VivoxVoiceManager_OnParticipantAddedEvent(string username, ChannelId channel, IParticipant participant)
        {
            if (channel.Name == HostSingleton.Instance.GameManager.JoinCode && participant.IsSelf)
            {
                // if joined the lobby channel and we're not hosting a match
                // we should request invites from hosts
            }
        }

        private static void OnUserLoggedIn()
        {
            var lobbyChannel =
                VivoxVoiceManager.Instance.ActiveChannels.FirstOrDefault(ac =>
                    ac.Channel.Name == HostSingleton.Instance.GameManager.JoinCode);

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
            VivoxVoiceManager.Instance.Logout();
        }

        public void ControlChatRoom()
        {
            if (chatRoomRectTr.localScale.y > .9f)
            {
                // 닫기
                chatRoomRectTr.DOScale(new Vector3(1f, 0f, 1f), .5f)
                    .SetEase(Ease.InBack);
            }
            else
            {
                chatRoomRectTr.DOScale(Vector3.one, .5f)
                    .SetEase(Ease.OutBack);
            }
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
        }
    }
}