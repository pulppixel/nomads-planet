using UnityEngine;

namespace NomadsPlanet
{
    public class HeadAnimPlayer : MonoBehaviour
    {
        [SerializeField] private AnimationClip blinkAnimationClip;

        private Animation _headAnimation;

        private void Awake()
        {
            _headAnimation = GetComponent<Animation>();
        }

        private void OnEnable()
        {
            _headAnimation.AddClip(blinkAnimationClip, blinkAnimationClip.name);
            _headAnimation.Play(blinkAnimationClip.name);
        }
    }
}