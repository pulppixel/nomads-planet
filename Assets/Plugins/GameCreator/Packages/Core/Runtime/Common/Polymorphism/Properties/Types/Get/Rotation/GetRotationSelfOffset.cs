using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Self with Offset")]
    [Category("Offsets/Self with Offset")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Yellow)]
    [Description("Rotation of the game object making the call in local or world space")]

    [Serializable] [HideLabelsInEditor]
    public class GetRotationSelfOffset : PropertyTypeGetRotation
    {
        [SerializeField] private RotationSpace m_Space = RotationSpace.Global;
        [SerializeField] private Vector3 m_Offset = Vector3.zero;
        
        public override Quaternion Get(Args args)
        {
            Quaternion offset = Quaternion.Euler(this.m_Offset);
            
            return args.Self != null 
                ? this.m_Space == RotationSpace.Global 
                    ? args.Self.transform.rotation * offset
                    : args.Self.transform.localRotation * offset
                : default;
        }

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationSelfOffset()
        );

        public override string String => $"{this.m_Space} Self";
    }
}