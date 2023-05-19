using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Last Footstep")]
    [Category("Characters/Character Last Footstep")]
    
    [Image(typeof(IconFootprint), ColorTheme.Type.Yellow)]
    [Description("The position and rotation of the Character's last step bone")]

    [Serializable]
    public class GetLocationCharacterLastFootstep : PropertyTypeGetLocation
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] private bool m_Rotate = true;
        
        public override Location Get(Args args)
        {
            Character character = m_Character.Get<Character>(args);

            return character != null && character.Footsteps.LastFootstep != null
                ? new Location(
                    character.Footsteps.LastFootstep.transform,
                    Space.Self, Vector3.zero, 
                    this.m_Rotate, Quaternion.identity
                ) : new Location();
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationCharacterLastFootstep()
        );

        public override string String => $"{this.m_Character} Last Footstep";
    }
}