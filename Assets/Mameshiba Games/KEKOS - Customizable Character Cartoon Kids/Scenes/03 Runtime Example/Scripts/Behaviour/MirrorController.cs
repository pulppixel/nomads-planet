using System.Collections;
using UnityEngine;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Behaviour
{
    public class MirrorController : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private float activatedFieldOfView;
        
        [SerializeField]
        private float activatedYLocalPosition;

        [SerializeField]
        private float zoomTime;
        
        [SerializeField]
        private GameObject mirrorCharacterModel;

        [SerializeField]
        private GameObject editorCanvas;
        
        [SerializeField]
        private GameObject controllersCanvas;

        [SerializeField]
        private SimpleCharacterController simpleCharacterController;

        private Transform _cameraTransform;
        private float _defaultCameraFieldOfView;
        private float _defaultCameraYLocalPosition;

        private void Awake()
        {
            _cameraTransform = mainCamera.transform;
            _defaultCameraFieldOfView = mainCamera.fieldOfView;
            _defaultCameraYLocalPosition = _cameraTransform.localPosition.y;
        }

        public void ActivateMirror()
        {
            StartCoroutine(InternalMirrorActivation(zoomTime));
        }

        private IEnumerator InternalMirrorActivation(float duration)
        {
            controllersCanvas.SetActive(false);
            
            simpleCharacterController.SetTargetForward(Vector3.forward);

            if (duration > 0)
            {
                float t = 0.0f;
                float startFieldOfView = mainCamera.fieldOfView;
                float endFieldOfView = activatedFieldOfView;

                Vector3 cameraPosition = _cameraTransform.localPosition;
                float startYPosition = cameraPosition.y;
                float endYPosition = activatedYLocalPosition;

                while (t < duration)
                {
                    t += Time.deltaTime;
                    mainCamera.fieldOfView = Mathf.Lerp(startFieldOfView, endFieldOfView, t / duration);
                    cameraPosition.y = Mathf.Lerp(startYPosition, endYPosition, t / duration);
                    _cameraTransform.localPosition = cameraPosition;
                    yield return null;
                }
                
                mainCamera.fieldOfView = endFieldOfView;
                cameraPosition.y = endYPosition;
                _cameraTransform.localPosition = cameraPosition;
            }

            Vector3 mirrorCharacterPosition = mirrorCharacterModel.transform.position;
            mirrorCharacterPosition.x = simpleCharacterController.transform.position.x;
            mirrorCharacterModel.transform.position = mirrorCharacterPosition;
            
            mirrorCharacterModel.SetActive(true);
            editorCanvas.SetActive(true);
        }
        
        public void DeactivateMirror()
        {
            StartCoroutine(InternalMirrorDeactivation(zoomTime));
        }
        
        private IEnumerator InternalMirrorDeactivation(float duration)
        {
            mirrorCharacterModel.SetActive(false);
            editorCanvas.SetActive(false);

            if (duration > 0)
            {
                float t = 0.0f;
                float startFieldOfView = mainCamera.fieldOfView;
                float endFieldOfView = _defaultCameraFieldOfView;

                Vector3 cameraPosition = _cameraTransform.localPosition;
                float startYPosition = cameraPosition.y;
                float endYPosition = _defaultCameraYLocalPosition;

                while (t < duration)
                {
                    t += Time.deltaTime;
                    mainCamera.fieldOfView = Mathf.Lerp(startFieldOfView, endFieldOfView, t / duration);
                    cameraPosition.y = Mathf.Lerp(startYPosition, endYPosition, t / duration);
                    _cameraTransform.localPosition = cameraPosition;
                    yield return null;
                }
                
                mainCamera.fieldOfView = endFieldOfView;
                cameraPosition.y = endYPosition;
                _cameraTransform.localPosition = cameraPosition;
            }
            
            controllersCanvas.SetActive(true);
            simpleCharacterController.EnableInput();
        }
    }
}