using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Towards Position")]
    [Category("Math/Towards Position")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Blue)]
    [Description("Rotation from a position towards another position in space")]

    [Serializable] [HideLabelsInEditor]
    public class GetRotationTowards : PropertyTypeGetRotation
    {
        [SerializeField] protected PropertyGetPosition m_From;
        [SerializeField] protected PropertyGetPosition m_Towards;
        
        public GetRotationTowards()
        {
            this.m_From = GetPositionSelfOffset.Create();
            this.m_Towards = GetPositionTargetOffset.Create();
        }

        public override Quaternion Get(Args args)
        {
            Vector3 source = this.m_From.Get(args);
            Vector3 target = this.m_Towards.Get(args);

            Vector3 direction = target - source;
            
            Vector3 flatDirection = Vector3.Scale(direction, Vector3Plane.NormalUp);
            if (flatDirection == Vector3.zero) return Quaternion.identity;
            
            return direction != Vector3.zero
                ? Quaternion.LookRotation(direction)
                : Quaternion.identity;
        }

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationTowards()
        );

        public override string String => $"Towards {this.m_Towards}";
    }
}