using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Player Child Index")]
    [Category("Characters/Player Child Index")]
    
    [Description("Reference to the N-th child of the Player game object")]
    [Image(typeof(IconPlayer), ColorTheme.Type.Green, typeof(OverlayDot))]

    [Serializable]
    public class GetGameObjectCharactersPlayerIndex : PropertyTypeGetGameObject
    {
        [SerializeField] 
        private PropertyGetInteger m_Index = new PropertyGetInteger();

        public override GameObject Get(Args args) => this.GetObject(args);

        private GameObject GetObject(Args args)
        {
            if (ShortcutPlayer.Instance == null) return null;
            Transform transform = ShortcutPlayer.Transform;
            
            int index = Mathf.Clamp((int) m_Index.Get(args), 0, transform.childCount - 1);
            Transform child = transform.GetChild(index);
            
            return child != null ? child.gameObject : null;
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCharactersPlayerIndex()
        );

        public override string String => $"Player/{this.m_Index}";
        
        public override GameObject SceneReference
        {
            get
            {
                if (!int.TryParse(this.m_Index.ToString(), out int index)) return null;
                Character[] instances = UnityEngine.Object.FindObjectsOfType<Character>();
                
                foreach (Character instance in instances)
                {
                    if (!instance.IsPlayer) continue;
                    
                    index = Math.Clamp(index, 0, instance.transform.childCount - 1);
                    return index < instance.transform.childCount 
                        ? instance.transform.GetChild(index).gameObject 
                        : null;
                }

                return null;
            }
        }
    }
}