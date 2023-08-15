using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.Vehicles.Car
{
    public class ButtonState : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool isPressed = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
        }
    }
}