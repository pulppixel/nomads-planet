using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Character Model")]
    [Category("Characters/Character Model")]
    
    [Description("Game Object that represents the model of a Character (under Mannequin)")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow, typeof(OverlayArrowDown))]

    [Serializable]
    public class GetGameObjectCharacterModel : PropertyTypeGetGameObject
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        public override GameObject Get(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Animim.Animator.gameObject : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            Character character = this.m_Character.Get<Character>(gameObject);
            return character != null && character.Animim.Animator != null
                ? character.Animim.Animator.gameObject
                : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectCharacterModel instance = new GetGameObjectCharacterModel();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"{this.m_Character} Model";
        
        public override GameObject SceneReference
        {
            get
            {
                GameObject reference = this.m_Character.SceneReference;
                if (reference == null) return null;

                Character character = reference.GetComponent<Character>();
                if (character == null) return null;

                return character.Animim.Animator != null
                    ? character.Animim.Animator.gameObject
                    : null;
            }
        }
    }
}