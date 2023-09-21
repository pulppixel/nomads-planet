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

        [SerializeField] private GameObject characterParent;

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
                interactionButton.image.rectTransform.DOScale(0f, .5f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => { interactionButton.interactable = false; });
            }
        }

        public void JoinThisMenu()
        {
            IsInteracting = true;
            blockImage.raycastTarget = false;
            interactionButton.image.rectTransform.DOScale(0f, .5f)
                .SetEase(Ease.InBack);
            userInfoRectTr.DOLocalMoveY(225f, .5f)
                .SetEase(Ease.InBack)
                .OnComplete(() => { characterParent.SetActive(false); });
            virtualCamera.Priority = 20;
        }

        public void LeaveThisMenu()
        {
            IsInteracting = false;
            blockImage.raycastTarget = true;
            virtualCamera.Priority = -1;

            characterParent.SetActive(true);
            userInfoRectTr.DOLocalMoveY(0f, .5f)
                .SetEase(Ease.OutBack);
        }
    }
}