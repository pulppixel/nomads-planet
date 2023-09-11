using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace NomadsPlanet
{
    public class ChatBubbleResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform bubbleRectTr;
        [SerializeField] private TMP_Text chatText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text nameText;

        private RectTransform _rectTransform;
        
        public float maxWidth = 110f;
        private const float PaddingTopBottom = 5f;
        private readonly Vector2 _paddingLeftRight = new Vector2(10, 10);

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetText(string message)
        {
            chatText.text = message;
            UpdateBubbleSize();
        }

        public void SetTime(string time)
        {
            timeText.text = time;
        }
        
        public void SetSender(string sender)
        {
            nameText.text = sender;
        }
        
        private void UpdateBubbleSize()
        {
            Vector2 size = chatText.GetPreferredValues(
                chatText.text,
                maxWidth - (_paddingLeftRight.x + _paddingLeftRight.y),
                0
            );

            bubbleRectTr.sizeDelta = new Vector2(
                Mathf.Min(size.x, maxWidth) + _paddingLeftRight.x + _paddingLeftRight.y,
                size.y + PaddingTopBottom * 2
            );

            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, bubbleRectTr.sizeDelta.y);
        }
    }
}