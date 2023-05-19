using MameshibaGames.Common.UI;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
#endif

namespace MameshibaGames.Kekos.CharacterEditorScene.Room
{
    public class RoomControler : MonoBehaviour
    {
        [SerializeField]
        private Transform[] elementsToRotate;

        [SerializeField]
        private float rotateSpeed;

        [SerializeField]
        private ContinuousButton rightButton;

        [SerializeField]
        private ContinuousButton leftButton;

        [HideInInspector]
        public float continuousDirection;

        private void Awake()
        {
            #if ENABLE_INPUT_SYSTEM
            StandaloneInputModule standaloneInputModule = FindObjectOfType<StandaloneInputModule>();
            GameObject standAloneGameObject = standaloneInputModule.gameObject;
            Destroy(standaloneInputModule);
            InputSystemUIInputModule inputSystem = standAloneGameObject.AddComponent<InputSystemUIInputModule>();
            inputSystem.enabled = false;
            inputSystem.enabled = true;
            #endif
        }

        private void Start()
        {
            leftButton.onDown.AddListener(() => continuousDirection = 1);
            rightButton.onDown.AddListener(() => continuousDirection = -1);
            leftButton.onUp.AddListener(() => continuousDirection = 0);
            rightButton.onUp.AddListener(() => continuousDirection = 0);
        }

        private void Update()
        {
            if (continuousDirection.Equals(0))
            {
                // Old input backends are enabled.
                #if ENABLE_LEGACY_INPUT_MANAGER
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                    InternalRotate(0, rotateSpeed, 0);
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    InternalRotate(0, -rotateSpeed, 0);
                // New input system backends are enabled.
                #elif ENABLE_INPUT_SYSTEM
                if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
                    InternalRotate(0, rotateSpeed, 0);
                if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
                    InternalRotate(0, -rotateSpeed, 0);
                #endif
            }
            else
            {
                InternalRotate(0, rotateSpeed * continuousDirection, 0);
            }

        }

        private void InternalRotate(float x, float y, float z)
        {
            foreach (Transform transform1 in elementsToRotate)
                transform1.Rotate(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
        }
    }
}
