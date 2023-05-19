using UnityEngine;
using UnityEngine.Events;

namespace MameshibaGames.Kekos.RuntimeExampleScene.Behaviour
{
    public class InteractElement : MonoBehaviour
    {
        [SerializeField]
        private Renderer neonRenderer;
        
        public UnityEvent onInteract = new UnityEvent();

        private static readonly int _Color = Shader.PropertyToID("_Color");
        private static readonly int _URPColor = Shader.PropertyToID("_BaseColor");
        private static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");

        private void Awake()
        {
            ExitElement();
            ChangeColor(new Color(0.37f, 0.05f, 1f));
        }

        public void Interact()
        {
            onInteract?.Invoke();
        }

        public void EnterElement()
        {
            ChangeColor(new Color(0.28f, 1f, 0.38f));
        }
        
        public void ExitElement()
        {
            ChangeColor(new Color(0.37f, 0.05f, 1f));
        }

        private void ChangeColor(Color newColor)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            neonRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(_Color, newColor);
            propertyBlock.SetColor(_URPColor, newColor);
            propertyBlock.SetColor(_EmissionColor, newColor);
            neonRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}