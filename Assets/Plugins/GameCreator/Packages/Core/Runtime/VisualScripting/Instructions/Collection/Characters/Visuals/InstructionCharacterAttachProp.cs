using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Attach Prop")]
    [Description("Attaches a prefab or instance Prop onto a Character's bone")]

    [Category("Characters/Visuals/Attach Prop")]

    [Parameter("Character", "The character target")]
    [Parameter("Type", "Whether to attach the prop as a prefab or instance")]
    [Parameter("Prop", "The prefab or instance object that is attached to the character")]
    [Parameter("Bone", "Which bone the prop is attached to")]
    [Parameter("Position", "Local offset from which the prop is distanced from the bone")]
    [Parameter("Rotation", "Local offset from which the prop is rotated from the bone")]

    [Keywords("Characters", "Add", "Grab", "Draw", "Pull", "Take", "Object")]
    [Image(typeof(IconTennis), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterAttachProp : Instruction
    {
        private enum Type
        {
            Prefab,
            Instance
        }

        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] private Type m_Type = Type.Prefab;
        [SerializeField] private PropertyGetGameObject m_Prop = new PropertyGetGameObject();

        [SerializeField] private Bone m_Bone = new Bone(HumanBodyBones.RightHand);
        
        [SerializeField] private Vector3 m_Position = Vector3.zero;
        [SerializeField] private Vector3 m_Rotation = Vector3.zero;

        public override string Title => $"Attach {this.m_Type} {this.m_Prop} on {this.m_Character} {this.m_Bone}";

        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            GameObject prop = this.m_Prop.Get(args);
            if (prop == null) return DefaultResult;

            switch (this.m_Type)
            {
                case Type.Prefab:
                    character.Props.AttachPrefab(
                        this.m_Bone, prop,
                        this.m_Position, Quaternion.Euler(this.m_Rotation)
                    );
                    break;
                
                case Type.Instance:
                    character.Props.AttachInstance(
                        this.m_Bone, prop,
                        this.m_Position, Quaternion.Euler(this.m_Rotation)
                    );
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }

            return DefaultResult;
        }
    }
}