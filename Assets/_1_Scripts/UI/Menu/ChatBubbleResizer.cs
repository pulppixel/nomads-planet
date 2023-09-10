using TMPro;
using UnityEngine;

namespace NomadsPlanet
{
    public class ChatBubbleResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform bubbleRectTr;
        [SerializeField] private TMP_Text chatText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text nameText;

        private const float MaxWidth = 110f;
        private const float PaddingTopBottom = 5f;
        private readonly Vector2 _paddingLeftRight = new Vector2(10, 10);

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
                MaxWidth - (_paddingLeftRight.x + _paddingLeftRight.y),
                0
            );

            bubbleRectTr.sizeDelta = new Vector2(
                Mathf.Min(size.x, MaxWidth) + _paddingLeftRight.x + _paddingLeftRight.y,
                size.y + PaddingTopBottom * 2
            );
        }
    }
}