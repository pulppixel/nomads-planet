using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace NomadsPlanet
{
    public class MenuInteraction : MonoBehaviour
    {
        [SerializeField] private Button interactionButton;
        [SerializeField] private RectTransform userInfoRectTr;
        [SerializeField] private Image blockImage;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        public static bool IsInteracting;
        private Collider _collider;

        private void Start()
        {
            interactionButton.image.rectTransform.localScale = Vector3.zero;
            blockImage.raycastTarget = true;
            virtualCamera.Priority = -1;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                interactionButton.interactable = true;
                interactionButton.image.rectTransform.DOScale(1f, .5f)
                    .SetEase(Ease.OutBack);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                interactionButton.interactable = false;
                interactionButton.image.rectTransform.DOScale(0f, .5f)
                    .SetEase(Ease.InBack);
            }
        }

        public void JoinThisMenu()
        {
            IsInteracting = true;
            blockImage.raycastTarget = false;
            interactionButton.image.rectTransform.DOScale(0f, .5f)
                .SetEase(Ease.InBack);
            userInfoRectTr.DOScale(0f, .5f)
                .SetEase(Ease.InBack);
            virtualCamera.Priority = 20;
        }
        
        public void LeaveThisMenu()
        {
            IsInteracting = false;
            blockImage.raycastTarget = true;
            virtualCamera.Priority = -1;
            
            userInfoRectTr.DOScale(1f, .5f)
                .SetEase(Ease.OutBack);
        }
    }
}