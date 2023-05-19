using MameshibaGames.Common.UI;
using UnityEngine;

namespace MameshibaGames.Common.Utils
{
    public class ChangeObjectPosition : MonoBehaviour
    {
        [SerializeField] 
        private Transform objectToChange;
        
        [SerializeField] 
        private float duration;
        
        [SerializeField] 
        private AnimationCurve scaleCurve;

        private Coroutine _lerpCoroutine;

        public void ChangePosition(Transform targetTransform)
        {
            if (_lerpCoroutine != null)
                StopCoroutine(_lerpCoroutine);
            
            _lerpCoroutine = StartCoroutine(objectToChange.LerpPosition(targetTransform.position, duration, scaleCurve));
        }
    }
}
