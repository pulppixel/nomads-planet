using System;
using MameshibaGames.Kekos.CharacterEditorScene.Room;
using UnityEngine;
using UnityEngine.UI;

namespace MameshibaGames.Kekos.CharacterEditorScene.UI
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private OffsetMaterial[] offsetMaterials;

        [SerializeField]
        private AnimationStructure[] animationStructures;

        [SerializeField]
        private GameObject contentObject;

        private AnimationStructure _latestAnimation;
        private AnimationStructure _poseAnimation;
        
        private static readonly int _Idle = Animator.StringToHash("Idle");

        private void Awake()
        {
            _poseAnimation = new AnimationStructure { triggerName = "Pose" };
            
            foreach (AnimationStructure animationStructure in animationStructures)
            {
                animationStructure.button.onClick.AddListener(() =>
                {
                    ChangeTrigger(animationStructure);
                });
            }
        }

        public void Hide()
        {
            ChangeTrigger(_poseAnimation, false);

            contentObject.SetActive(false);
        }

        public void Show()
        {
            contentObject.SetActive(true);

            if (_latestAnimation == null) return;
            ChangeTrigger(_latestAnimation, force: true);
        }

        private void ChangeTrigger(AnimationStructure animationStructure, bool savePose = true, bool force = false)
        {
            if (_latestAnimation == animationStructure && !force) return;
            
            if (savePose)
                _latestAnimation = animationStructure;
            
            animator.ResetTrigger(_Idle);
            animator.SetTrigger(animationStructure.triggerName);
            foreach (OffsetMaterial offsetMaterial in offsetMaterials)
                offsetMaterial.offsetSpeed = animationStructure.offsetSpeed;
        }

        [Serializable]
        public class AnimationStructure
        {
            public Button button;
            public string triggerName;
            public Vector2 offsetSpeed;
        }
    }
}