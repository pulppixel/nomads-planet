using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MameshibaGames.Kekos.CharacterEditorScene.Room
{
    public class CharacterRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,
        IPointerDownHandler
    {
        [SerializeField]
        private Texture2D cursorTexture;

        [SerializeField]
        private CursorMode cursorMode;

        private bool _isDragging;
        private bool _isInside;

        [SerializeField]
        private RoomControler roomControler;

        [SerializeField]
        private float rotateSpeed;

        private Vector3 _prevPosition;

        private void Awake()
        {
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        private void Update()
        {
            if (!_isDragging) return;

            #if ENABLE_LEGACY_INPUT_MANAGER
            Vector2 mouseDelta = Input.mousePosition - _prevPosition;
            _prevPosition = Input.mousePosition;
            #elif ENABLE_INPUT_SYSTEM
            Vector2 mouseDelta = Mouse.current.position.ReadValue() - (Vector2)_prevPosition;
            _prevPosition = Mouse.current.position.ReadValue();
            #endif
            
            roomControler.continuousDirection = -mouseDelta.x * rotateSpeed;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Cursor.SetCursor(cursorTexture, Vector2.zero, cursorMode);
            _isInside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isInside = false;
            if (!_isDragging && !_isInside)
                Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
            if (!_isDragging && !_isInside)
                Cursor.SetCursor(null, Vector2.zero, cursorMode);
            roomControler.continuousDirection = 0;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            #if ENABLE_LEGACY_INPUT_MANAGER
            _prevPosition = Input.mousePosition;
            #elif ENABLE_INPUT_SYSTEM
            _prevPosition = Mouse.current.position.ReadValue();
            #endif
            _isDragging = true;
        }
    }
}
