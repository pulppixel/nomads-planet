using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.Vehicles.Car
{
    public class ButtonState : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool isPressed = false;

        private Image thisImg;

        private void Awake()
        {
            thisImg = GetComponent<Image>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            thisImg.color = Color.grey;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            thisImg.color = Color.white;
        }
    }
}