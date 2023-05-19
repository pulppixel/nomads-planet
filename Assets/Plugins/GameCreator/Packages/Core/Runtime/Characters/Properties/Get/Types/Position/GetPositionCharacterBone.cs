using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Bone")]
    [Category("Characters/Character Bone")]
    
    [Image(typeof(IconBoneSolid), ColorTheme.Type.Yellow)]
    [Description("Returns the position of the Character's bone")]

    [Serializable]
    public class GetPositionCharacterBone : PropertyTypeGetPosition
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        [SerializeField] protected Bone m_Bone = new Bone(HumanBodyBones.RightHand);

        public GetPositionCharacterBone()
        { }

        public GetPositionCharacterBone(Character character, HumanBodyBones humanBone)
        {
            this.m_Character = GetGameObjectCharactersInstance.CreateWith(character);
            this.m_Bone = new Bone(humanBone);
        }

        public override Vector3 Get(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return default;

            Transform boneTransform = this.m_Bone.GetTransform(character.Animim?.Animator); 
            return boneTransform != null ? boneTransform.position : default;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionCharacterBone()
        );
        
        public static PropertyGetPosition CreateWith(Character character, HumanBodyBones humanBone)
        {
            return new PropertyGetPosition(
                new GetPositionCharacterBone(character, humanBone)
            );
        }

        public override string String => $"{this.m_Character}/{this.m_Bone}";
    }
}