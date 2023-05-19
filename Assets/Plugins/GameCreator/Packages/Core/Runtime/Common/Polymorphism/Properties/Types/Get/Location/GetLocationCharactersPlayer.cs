using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Player")]
    [Category("Characters/Player")]
    
    [Image(typeof(IconPlayer), ColorTheme.Type.Green)]
    [Description("The position and rotation of the Player")]

    [Serializable]
    public class GetLocationCharactersPlayer : PropertyTypeGetLocation
    {
        [SerializeField] private bool m_Rotate = true;
        
        public override Location Get(Args args)
        {
            return new Location(ShortcutPlayer.Transform, Space.Self, Vector3.zero, this.m_Rotate, Quaternion.identity);
        }

        public override Location Get(GameObject gameObject)
        {
            return new Location(ShortcutPlayer.Transform, Space.Self, Vector3.zero, this.m_Rotate, Quaternion.identity);
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationCharactersPlayer()
        );

        public override string String => "Player";
    }
}