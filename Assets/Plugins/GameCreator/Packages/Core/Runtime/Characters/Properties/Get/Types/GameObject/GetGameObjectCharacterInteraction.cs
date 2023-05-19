using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Interaction")]
    [Category("Characters/Character Interaction")]
    
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Yellow)]
    [Description("Reference to the Interactive element selected by a Character")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectCharacterInteraction : PropertyTypeGetGameObject
    {
        [SerializeField] protected Character m_Character;

        public override GameObject Get(Args args)
        {
            return this.m_Character != null 
                ? this.m_Character.Interaction.Target?.Instance 
                : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return this.m_Character != null 
                ? this.m_Character.Interaction.Target?.Instance 
                : null;
        }

        public override string String => string.Format(
            "{0} Interaction",
            this.m_Character != null ? this.m_Character.gameObject.name : "(none)"
        );
    }
}