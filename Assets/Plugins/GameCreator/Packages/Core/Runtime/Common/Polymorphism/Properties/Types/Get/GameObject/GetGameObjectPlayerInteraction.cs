using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Player Interaction")]
    [Category("Characters/Player Interaction")]
    
    [Description("The Interactive element currently selected by the Player")]
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Green)]

    [Serializable]
    public class GetGameObjectPlayerInteraction : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return ShortcutPlayer.Instance != null 
                ? ShortcutPlayer.Get<Character>().Interaction.Target?.Instance 
                : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return ShortcutPlayer.Instance != null 
                ? ShortcutPlayer.Get<Character>().Interaction.Target?.Instance 
                : null;
        }

        public override string String => "Player Interaction";
    }
}