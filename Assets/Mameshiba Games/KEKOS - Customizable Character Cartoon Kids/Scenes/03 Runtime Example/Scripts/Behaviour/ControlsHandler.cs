using MameshibaGames.Common.UI;
using MameshibaGames.Kekos.RuntimeExampleScene.Behaviour;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
#endif

public class ControlsHandler : MonoBehaviour
{
    [SerializeField]
    private SimpleCharacterController simpleCharacterController;

    [SerializeField]
    private ContinuousButton leftButton, rightButton, downButton, upButton, sprintButton, jumpButton, interactButton;

    [SerializeField]
    private GameObject windowsContainer;

    [SerializeField]
    private GameObject mobileContainer;
    
    private void Awake()
    {
        #if ENABLE_INPUT_SYSTEM && UNITY_EDITOR
        StandaloneInputModule standaloneInputModule = FindObjectOfType<StandaloneInputModule>();
        GameObject standAloneGameObject = standaloneInputModule.gameObject;
        Destroy(standaloneInputModule);
        InputSystemUIInputModule inputSystem = standAloneGameObject.AddComponent<InputSystemUIInputModule>();
        inputSystem.enabled = false;
        inputSystem.enabled = true;
        #endif
        
        #if UNITY_STANDALONE
        mobileContainer.SetActive(false);
        return;
        #endif
        
        mobileContainer.SetActive(true);
        windowsContainer.SetActive(false);
        
        leftButton.onDown.AddListener(() =>
        {
            simpleCharacterController.left = true;
        });
        leftButton.onUp.AddListener(() =>
        {
            simpleCharacterController.left = false;
        });
        rightButton.onDown.AddListener(() =>
        {
            simpleCharacterController.right = true;
        });
        rightButton.onUp.AddListener(() =>
        {
            simpleCharacterController.right = false;
        });
        downButton.onDown.AddListener(() =>
        {
            simpleCharacterController.down = true;
        });
        downButton.onUp.AddListener(() =>
        {
            simpleCharacterController.down = false;
        });
        upButton.onDown.AddListener(() =>
        {
            simpleCharacterController.up = true;
        });
        upButton.onUp.AddListener(() =>
        {
            simpleCharacterController.up = false;
        });
        sprintButton.onDown.AddListener(() =>
        {
            simpleCharacterController.sprint = true;
        });
        sprintButton.onUp.AddListener(() =>
        {
            simpleCharacterController.sprint = false;
        });
        
        jumpButton.onDown.AddListener(() =>
        {
            simpleCharacterController.jump = true;
        });
        jumpButton.onUp.AddListener(() =>
        {
            simpleCharacterController.jump = false;
        });
        
        interactButton.onDown.AddListener(() =>
        {
            simpleCharacterController.interact = true;
        });
        interactButton.onUp.AddListener(() =>
        {
            simpleCharacterController.interact = false;
        });
    }

    private void Update()
    {
        #if UNITY_STANDALONE
        CheckInputs();
        #endif
    }
    
    private void CheckInputs()
    {
        // Old input backends are enabled.
        #if ENABLE_LEGACY_INPUT_MANAGER
        simpleCharacterController.jump = Input.GetKeyDown(KeyCode.Space);
        simpleCharacterController.interact = Input.GetKeyDown(KeyCode.F);
        simpleCharacterController.left = Input.GetKey(KeyCode.A);
        simpleCharacterController.right = Input.GetKey(KeyCode.D);
        simpleCharacterController.up = Input.GetKey(KeyCode.W);
        simpleCharacterController.down = Input.GetKey(KeyCode.S);
        simpleCharacterController.sprint = Input.GetKey(KeyCode.LeftShift);
            
        // New input system backends are enabled.
        #elif ENABLE_INPUT_SYSTEM
        simpleCharacterController.jump = Keyboard.current.spaceKey.wasPressedThisFrame;
        simpleCharacterController.interact = Keyboard.current.fKey.wasPressedThisFrame;
        simpleCharacterController.left = Keyboard.current.aKey.isPressed;
        simpleCharacterController.right = Keyboard.current.dKey.isPressed;
        simpleCharacterController.up = Keyboard.current.wKey.isPressed;
        simpleCharacterController.down = Keyboard.current.sKey.isPressed;
        simpleCharacterController.sprint = Keyboard.current.leftShiftKey.isPressed;
        #endif
    }
}
