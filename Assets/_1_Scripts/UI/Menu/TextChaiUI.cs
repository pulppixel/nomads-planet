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
    public class TextChaiUI : MonoBehaviour
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

#if UNITY_SERVER
            sendButton.gameObject.SetActive(false);
            messageInputField.gameObject.SetActive(false);
#else
            sendButton.onClick.AddListener(SubmitTextToVivox);
            messageInputField.onEndEdit.AddListener(_ => { EnterKeyOnTextField(); });
#endif
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => VivoxVoiceManager.Instance.LoginState == LoginState.LoggedIn);

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
            CustomFunc.ConsoleLog("OnParticipantAdded!!");
        }

        private void OnTextMessageLogReceivedEvent(string sender, IChannelTextMessage channeltextmessage)
        {
            if (!string.IsNullOrEmpty(channeltextmessage.ApplicationStanzaNamespace))
            {
                return;
            }

            if (channeltextmessage.FromSelf)
            {
                var newMessageObj = Instantiate(chatContentObj, messageParent);
                _messageObjPool.Add(newMessageObj);
                newMessageObj.SetText(channeltextmessage.Message);
                newMessageObj.SetTime(channeltextmessage.ReceivedTime.ToString("tt h:mm"));
                StartCoroutine(SendScrollRectToBottom());
            }
            else
            {
                var newMessageObj = Instantiate(chatOtherObj, messageParent);
                _messageObjPool.Add(newMessageObj);
                newMessageObj.SetText(channeltextmessage.Message);
                newMessageObj.SetTime(channeltextmessage.ReceivedTime.ToString("tt h:mm"));
                newMessageObj.SetSender(sender);
            }
        }

        private void EnterKeyOnTextField()
        {
            if (!Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }

            SubmitTextToVivox();
            CustomFunc.ConsoleLog("Submit Text!!");
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

            _textChatScrollRect.normalizedPosition = new Vector2(0, 0);

            yield return null;
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