using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NomadsPlanet.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;

namespace NomadsPlanet
{
    public class TextChatUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField messageInputField;
        [SerializeField] private Transform messageParent;
        [SerializeField] private ChatBubbleResizer chatContentObj;
        [SerializeField] private ChatBubbleResizer chatOtherObj;
        [SerializeField] private Button sendButton;

        private ScrollRect _textChatScrollRect;
        private ChannelId _lobbyChannelId;
        private readonly List<ChatBubbleResizer> _messageObjPool = new List<ChatBubbleResizer>();

        private void Awake()
        {
            _textChatScrollRect = GetComponent<ScrollRect>();
            if (_messageObjPool.Count > 0)
            {
                ClearMessageObjectPool();
            }

            ClearOutTextField();

            VivoxVoiceManager.Instance.OnParticipantAddedEvent += OnParticipantAdded;
            VivoxVoiceManager.Instance.OnTextMessageLogReceivedEvent += OnTextMessageLogReceivedEvent;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => VivoxVoiceManager.Instance.LoginState == LoginState.LoggedIn);

#if UNITY_SERVER
            sendButton.gameObject.SetActive(false);
            messageInputField.gameObject.SetActive(false);
#else
            sendButton.onClick.AddListener(SubmitTextToVivox);
            messageInputField.onEndEdit.AddListener(_ => { EnterKeyOnTextField(); });
#endif

            if (VivoxVoiceManager.Instance.ActiveChannels.Count > 0)
            {
                _lobbyChannelId = VivoxVoiceManager.Instance.ActiveChannels
                    .FirstOrDefault(ac => ac.Channel.Name == SceneName.MenuScene)
                    ?.Key;
            }
        }


        private void OnParticipantAdded(string username, ChannelId channel, IParticipant participant)
        {
            if (VivoxVoiceManager.Instance.ActiveChannels.Count > 0)
            {
                _lobbyChannelId = VivoxVoiceManager.Instance.ActiveChannels.FirstOrDefault()?.Channel;
            }
        }

        private void OnTextMessageLogReceivedEvent(string sender, IChannelTextMessage channelTextMessage)
        {
            if (!string.IsNullOrEmpty(channelTextMessage.ApplicationStanzaNamespace))
            {
                return;
            }

            var newMessageObj = Instantiate(
                channelTextMessage.FromSelf ? chatContentObj : chatOtherObj,
                messageParent
            );

            _messageObjPool.Add(newMessageObj);
            newMessageObj.SetText(channelTextMessage.Message);
            newMessageObj.SetTime(channelTextMessage.ReceivedTime.ToString("tt h:mm"));
            newMessageObj.SetSender(sender);

            StartCoroutine(SendScrollRectToBottom());
        }

        private void EnterKeyOnTextField()
        {
            if (!Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }

            SubmitTextToVivox();
        }

        private void SubmitTextToVivox()
        {
            if (string.IsNullOrEmpty(messageInputField.text))
            {
                return;
            }

            VivoxVoiceManager.Instance.SendTextMessage(messageInputField.text, _lobbyChannelId);
            ClearOutTextField();
        }

        private void ClearOutTextField()
        {
            messageInputField.text = string.Empty;
            messageInputField.Select();
            messageInputField.ActivateInputField();
        }

        private void ClearMessageObjectPool()
        {
            foreach (var message in _messageObjPool)
            {
                Destroy(message);
            }

            _messageObjPool.Clear();
        }


        private IEnumerator SendScrollRectToBottom()
        {
            yield return new WaitForEndOfFrame();

            LayoutRebuilder.ForceRebuildLayoutImmediate(_textChatScrollRect.content);

            yield return null;

            _textChatScrollRect.verticalNormalizedPosition = 0f;
        }

        private void OnDestroy()
        {
            VivoxVoiceManager.Instance.OnParticipantAddedEvent -= OnParticipantAdded;
            VivoxVoiceManager.Instance.OnTextMessageLogReceivedEvent -= OnTextMessageLogReceivedEvent;

#if !UNITY_SERVER
            sendButton.onClick.RemoveAllListeners();
            messageInputField.onEndEdit.RemoveAllListeners();
#endif
        }
    }
}