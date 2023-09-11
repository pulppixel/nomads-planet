using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.Vehicles.Car
{
    public class ButtonState : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool isPressed = false;

        private Image _thisImg;

        private void Awake()
        {
            _thisImg = GetComponent<Image>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            _thisImg.color = Color.grey;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            _thisImg.color = Color.white;
        }
    }
}